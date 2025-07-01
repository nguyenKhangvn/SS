using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDto>> GetAllAsync(Guid userId);
        Task<PaymentDto?> GetByIdAsync(Guid id);
        Task<PaymentDto?> GetByOrderIdAsync(Guid orderId);
        Task<PaymentDto> CreateAsync(PaymentDto dto);
        Task<PaymentDto?> UpdateAsync(PaymentDto dto);
        Task<bool> DeleteAsync(Guid id);

    }
}
