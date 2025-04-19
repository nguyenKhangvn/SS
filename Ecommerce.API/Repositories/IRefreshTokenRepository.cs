namespace Ecommerce.API.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> CreateAsync(RefreshToken token);
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task RevokeAsync(Guid tokenId, string replacedByToken = null);
        Task RevokeAllForUserAsync(Guid userId);
        Task<bool> IsActive(Guid tokenId);
        Task UpdateAsync(RefreshToken token);
        Task<RefreshToken?> GetValidTokenAsync(string token, Guid userId);
    }
}
