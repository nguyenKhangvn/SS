namespace Ecommerce.API.Repositories.Interfaces
{
    public interface IStoreLocationRepository
    {
        Task<IEnumerable<StoreLocation>> GetStoreLocationsAsync(string? includeProperties = null);
        Task<StoreLocation?> GetByIdAsync(Guid id, string? includeProperties = null);
        Task<StoreLocation> AddAsync(StoreLocation storeLocation);
        Task<StoreLocation> UpdateAsync(StoreLocation storeLocation);
        Task<bool> DeleteAsync(Guid id);
    }
}
