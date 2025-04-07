using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services.Interfaces
{
    public interface IStoreLocationService
    {
        Task<IEnumerable<StoreLocationDto>> GetStoreLocations(string? includeProperties = null);
        Task<StoreLocationDto?> GetByIdAsync(Guid id, string includeProperties);
        Task<StoreLocationDto> AddAsync(StoreLocationDto storeLocation);
        Task<StoreLocationDto> UpdateAsync(StoreLocationDto storeLocation);
        Task<bool> DeleteAsync(Guid id);
    }
}
