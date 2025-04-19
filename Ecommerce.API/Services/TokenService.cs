using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Ecommerce.API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _signingKey;

        public TokenService(IConfiguration config)
        {
            _config = config;
            _signingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        }
        public string GenerateJwtToken(User user)
        {
            var claims = new[] //thông tin người dùng sẽ được mã hóa trong token
             {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = GetAccessTokenExpiry(),
                SigningCredentials = new SigningCredentials(
                    _signingKey,
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
            //hoặc return Guid.NewGuid().ToString();
        }

        public DateTime GetAccessTokenExpiry()
        {
            var expiryInMinutes = _config.GetValue<int>("Jwt:ExpiryInMinutes", 30);
            return DateTime.UtcNow.AddMinutes(expiryInMinutes);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)//Lấy thông tin người dùng từ token đã hết hạn phục vụ cấp 1 token mới mà k cần đăng nhập lại
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _config["Jwt:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,
                ValidateLifetime = false // Cho phép token đã hết hạn
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            return principal;
        }

        public ClaimsPrincipal? ValidateToken(string token) //ClaimsPrincipal nếu token hợp lệ thì sẽ chứa thông tin người dùng
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters //giải mã và xác thực token
                {
                    ValidateIssuer = true, //bên cấp token đúng k
                    ValidIssuer = _config["Jwt:Issuer"], // lấy từ cấu hình (issuer hợp lệ)
                    ValidateAudience = true, // người nhận token đúng k
                    ValidAudience = _config["Jwt:Audience"], // lấy từ cấu hình (audience hợp lệ)
                    ValidateIssuerSigningKey = true, // kiểm tra chữ ký secret key
                    IssuerSigningKey = _signingKey, // lấy từ cấu hình (secret key hợp lệ)
                    ValidateLifetime = true, // kiểm tra thời gian hết hạn token
                    ClockSkew = TimeSpan.Zero // không cho phép thời gian trễ
                }, out _);

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
