namespace Ecommerce.API.Repositories
{
    public interface IStoreLocationRepository
    {
        IQueryable<StoreLocation> GetAll();
        Task<StoreLocation?> GetByIdAsync(Guid id, string? includeProperties = null);
        Task<StoreLocation> AddAsync(StoreLocation storeLocation);
        Task<StoreLocation> UpdateAsync(StoreLocation storeLocation);
        Task<bool> DeleteAsync(Guid id);
    }
}
