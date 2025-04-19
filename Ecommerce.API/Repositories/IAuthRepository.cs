
namespace Ecommerce.API.Repositories
{
    public interface IAuthRepository
    {
 
        //Task AddUserAsync(User user);
        Task<RefreshToken> AddRefreshTokenAsync(Guid userId, string token, DateTime expires);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task RevokeRefreshTokenAsync(string token);
        //Task<User?> GetUserByIdAsync(Guid userId);
    }
}
