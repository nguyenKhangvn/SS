using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services.Interfaces
{
    public interface IManufacturerService
    {
        Task<IEnumerable<ManufacturerDto>> GetAllAsync();
        Task<ManufacturerDto?> GetByIdAsync(Guid id);
        Task<ManufacturerDto> CreateAsync(ManufacturerDto dto);
        Task<ManufacturerDto?> UpdateAsync(Guid id, ManufacturerDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
