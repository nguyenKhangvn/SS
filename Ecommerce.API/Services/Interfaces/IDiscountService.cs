using Ecommerce.Infrastructure.Models.Dtos;
namespace Ecommerce.API.Services.Interfaces
{
    public interface IDiscountService
    {
        Task<IEnumerable<DiscountDto>> GetAllAsync();
        Task<DiscountDto?> GetByIdAsync(Guid id);
        Task<DiscountDto> CreateAsync(DiscountDto dto);
        Task<bool> UpdateAsync(Guid id, DiscountDto dto);
        Task<bool> DeleteAsync(Guid id);

    }
}
