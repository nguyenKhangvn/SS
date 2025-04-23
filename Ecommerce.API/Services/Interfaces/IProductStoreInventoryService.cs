using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services.Interfaces
{
    public interface IProductStoreInventoryService
    {
        Task<ProductStoreInventoryDto> GetByIdAsync(Guid id);
        Task<IEnumerable<ProductStoreInventoryDto>> GetByProductIdAsync(Guid productId);
        Task<IEnumerable<ProductStoreInventoryDto>> GetByStoreIdAsync(Guid storeId);
        Task<ProductStoreInventoryDto?> GetByProductAndStoreAsync(Guid productId, Guid storeId);
        Task<ProductStoreInventoryDto> AddAsync(AddOrUpdateProductStoreInventoryDto dto);
        Task<ProductStoreInventoryDto> UpdateAsync(Guid id, AddOrUpdateProductStoreInventoryDto dto);
        Task UpdateQuantityAsync(Guid productId, Guid storeId, int quantityChange);
        Task DeleteAsync(Guid id);
    }
}
