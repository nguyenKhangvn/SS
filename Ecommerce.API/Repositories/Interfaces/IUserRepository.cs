namespace Ecommerce.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync(string? includeProperties = null);
        Task<User?> GetByIdAsync(Guid id, string? includeProperties = null);
        Task<User> AddAsync(User user);
        Task<User?> UpdateAsync(User user);
        Task<bool> DeleteAsync(Guid id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetByIdsAsync(List<Guid> ids);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
        Task<IEnumerable<User>> GetUsersByRolesAdminOrStaffAsync(IEnumerable<string> roles);
        Task<User?> FindAvailableStaffAsync();

    }
}
