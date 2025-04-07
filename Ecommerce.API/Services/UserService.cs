using AutoMapper;
using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.API.Services
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository, IMapper mapper, PasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }
        public async Task<UserDto?> AddAsync(UserCreateDto dto)
        {
            var existingUser = (await _userRepository.GetAllAsync()).FirstOrDefault(user => user.Email == dto.Email);
            if (existingUser != null)
            {
                return null; // Người dùng đã tồn tại
            }
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Role = Enum.Parse<RoleStatus>(dto.Role, true),
                IsActive = dto.IsActive,
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
            var created = await _userRepository.AddAsync(user);
            return _mapper.Map<UserDto>(created);
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _userRepository.DeleteAsync(id);
        }
        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }
        public async Task<UserDto?> GetByIdAsync(Guid id, string? includeProperties)
        {
            var user = await _userRepository.GetByIdAsync(id, includeProperties);
            if (user == null)
            {
                return null;
            }
            return _mapper.Map<UserDto>(user);
        }
        public async Task<UserDto?> UpdateAsync(Guid id, UserCreateDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            user.Name = dto.Name;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            user.Role = Enum.Parse<RoleStatus>(dto.Role, true);
            user.IsActive = dto.IsActive;

            // Nếu cập nhật mật khẩu (nếu có truyền vào)
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
            }

            var updatedUser = await _userRepository.UpdateAsync(user);
            return _mapper.Map<UserDto>(updatedUser);
        }
        public async Task<UserDto?> LoginAsync(UserLoginDto dto)
        {
            var user = (await _userRepository.GetAllAsync()).FirstOrDefault(u => u.Email == dto.Email);
            if (user == null) return null;

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed) return null;

            return _mapper.Map<UserDto>(user);
        }
        public async Task<UserDto?> RegisterAsync(UserCreateDto dto)
        {
            var existing = (await _userRepository.GetAllAsync())
                .FirstOrDefault(u => u.Email == dto.Email);
            if (existing != null) return null; // Email đã tồn tại

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber,
                Role = Enum.Parse<RoleStatus>(dto.Role, true),
                IsActive = dto.IsActive
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
            var created = await _userRepository.AddAsync(user);
            return _mapper.Map<UserDto>(created);
        }

    }
}
