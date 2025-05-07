namespace Ecommerce.API.Repositories.Interfaces
{
    public interface IProductStoreInventoryRepository
    {
        Task<ProductStoreInventory?> GetByIdAsync(Guid id);
        Task<IEnumerable<ProductStoreInventory>> GetByProductIdAsync(Guid productId);
        Task<IEnumerable<ProductStoreInventory>> GetByStoreIdAsync(Guid storeId);
        Task<ProductStoreInventory?> GetByProductAndStoreAsync(Guid productId, Guid storeId);
        Task AddAsync(ProductStoreInventory inventory);
        Task UpdateAsync(ProductStoreInventory inventory);
        Task DeleteAsync(ProductStoreInventory inventory);
    }
}
