using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(Guid id, string? includeProperties);
        Task<UserDto> AddAsync(UserCreateDto dto);
        Task<UserDto?> UpdateAsync(Guid id, UserCreateDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<UserDto?> LoginAsync(UserLoginDto dto);
        Task<UserDto?> RegisterAsync(UserCreateDto dto);
    }
}
