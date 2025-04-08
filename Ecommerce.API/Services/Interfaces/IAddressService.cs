using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services.Interfaces
{
    public interface IAddressService
    {
        Task<IEnumerable<AddressDto>> GetAllAsync();
        Task<AddressDto?> GetByIdAsync(Guid id);
        Task<AddressDto> CreateAsync(AddressDto dto);
        Task<AddressDto?> UpdateAsync(AddressDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
