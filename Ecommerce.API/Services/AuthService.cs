using Ecommerce.Infrastructure.Entity;
using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using System.Security.Claims;
using static Ecommerce.Infrastructure.Models.Dtos.AuthDto;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Ecommerce.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepo;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly ITokenService _tokenService;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public AuthService(
            IAuthRepository authRepo,
            IRefreshTokenRepository refreshTokenRepo,
            ITokenService tokenService,
            PasswordHasher<User> passwordHasher,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            IUserRepository userRepository)
        {
            _authRepo = authRepo;
            _refreshTokenRepo = refreshTokenRepo;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            // xet authen
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            // tao token
            var accessToken = _tokenService.GenerateJwtToken(user);
            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(1440);

            var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var refreshToken = await CreateRefreshToken(user.Id, ipAddress);

            // chi luu token ở cookie
            if (_httpContextAccessor.HttpContext != null)
            {
                _httpContextAccessor.HttpContext.Response.Cookies.Append(
                    "refresh_token",
                    refreshToken.Token,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Lax,
                        Expires = refreshToken.Expires,
                        Path = "/"
                    });
            }

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                Expiry = accessTokenExpiry,
                User = _mapper.Map<UserDto>(user)
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(UserCreateDto dto)
        {
            if (await _userRepository.GetUserByEmailAsync(dto.Email) != null)
                throw new ArgumentException("Email already exists");

            var user = new User
            {
                Email = dto.Email,
                Name = dto.Name,
                Role = Enum.Parse<RoleStatus>(dto.Role),
                PasswordHash = _passwordHasher.HashPassword(null, dto.Password)
            };

            await _userRepository.AddAsync(user);

            var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
            var refreshToken = await CreateRefreshToken(user.Id, ipAddress);

            return new AuthResponseDto
            {
                AccessToken = _tokenService.GenerateJwtToken(user),
                RefreshToken = refreshToken.Token,
                Expiry = refreshToken.Expires,
                User = _mapper.Map<UserDto>(user)
            };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(dto.AccessToken);
            if (principal == null)
                throw new SecurityTokenException("Invalid access token");

            var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                throw new SecurityTokenException("Invalid token: Missing user ID");

            var userId = Guid.Parse(userIdClaim);

            var storedToken = await _refreshTokenRepo.GetValidTokenAsync(dto.RefreshToken, userId);
            if (storedToken == null || storedToken.IsRevoked || storedToken.IsExpired)
                throw new SecurityTokenException("Invalid refresh token");

            storedToken.Revoked = DateTime.UtcNow;
            storedToken.ReplacedByToken = dto.RefreshToken;
            await _refreshTokenRepo.UpdateAsync(storedToken);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new SecurityTokenException("User not found");

            var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
            var newRefreshToken = await CreateRefreshToken(user.Id, ipAddress);

            return new AuthResponseDto
            {
                AccessToken = _tokenService.GenerateJwtToken(user),
                RefreshToken = newRefreshToken.Token,
                Expiry = newRefreshToken.Expires,
                User = _mapper.Map<UserDto>(user)
            };
        }


        public async Task RevokeTokenAsync(string refreshToken)
        {
            var token = await _refreshTokenRepo.GetByTokenAsync(refreshToken);
            if (token == null) return;

            token.Revoked = DateTime.UtcNow;
            await _refreshTokenRepo.UpdateAsync(token);
        }

        private async Task<RefreshToken> CreateRefreshToken(Guid userId, string ipAddress)
        {
            var expiryDays = _configuration.GetValue<int>("Jwt:RefreshTokenExpiryInDays", 7);
            var refreshToken = new RefreshToken
            {
                Token = _tokenService.GenerateRefreshToken(),
                UserId = userId,
                Expires = DateTime.UtcNow.AddDays(expiryDays),
                CreatedByIp = ipAddress,
                DeviceInfo = GetDeviceInfo()
            };

            await _refreshTokenRepo.CreateAsync(refreshToken);
            return refreshToken;
        }

        public async Task<AuthResponseDto> LoginWithGoogleAsync(string email, string name)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));

            // Tìm user theo email
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
            {
                // Nếu chưa có user → auto đăng ký
                user = new User
                {
                    Email = email,
                    Name = name,
                    PasswordHash = "", // Có thể bỏ trống hoặc đặt flag isGoogleUser
                    CreatedAt = DateTime.UtcNow
                };
                await _userRepository.AddAsync(user);
            }

            // Giống phần Login bình thường: tạo token, refresh token
            var accessToken = _tokenService.GenerateJwtToken(user);
            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(1440);

            var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var refreshToken = await CreateRefreshToken(user.Id, ipAddress);

            if (_httpContextAccessor.HttpContext != null)
            {
                _httpContextAccessor.HttpContext.Response.Cookies.Append(
                    "refresh_token",
                    refreshToken.Token,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Lax,
                        Expires = refreshToken.Expires,
                        Path = "/"
                    });
            }

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                Expiry = accessTokenExpiry,
                User = _mapper.Map<UserDto>(user)
            };
        }




        private string GetDeviceInfo()
        {
            var userAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();
            // Implement proper device parsing logic here
            return userAgent ?? "Unknown";
        }

        public Task RequestPasswordResetAsync(string email)
        {
            //var user = await _userRepository.GetUserByEmailAsync(email);
            //if (user == null) return;
            throw new NotImplementedException();
        }

        public Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
        {
            throw new NotImplementedException();
        }
    }
}