using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services.Interfaces
{
    public interface IOrderItemService
    {
        Task<IEnumerable<OrderItemDto>> GetAllAsync();
        Task<OrderItemDto?> GetByIdAsync(Guid id);
        Task<OrderItemDto> CreateAsync(OrderItemDto dto);
        Task<OrderItemDto?> UpdateAsync(Guid id, OrderItemDto dto);
        Task<bool> DeleteAsync(Guid id);

    }
}
