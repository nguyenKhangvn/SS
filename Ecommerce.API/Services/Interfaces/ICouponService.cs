using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services.Interfaces
{
    public interface ICouponService
    {
        Task<IEnumerable<CouponDto>> GetAllAsync();
        Task<CouponDto?> GetByIdAsync(Guid id);
        Task<CouponDto> AddAsync(CouponDto coupon);
        Task<CouponDto> UpdateAsync(CouponDto coupon);
        Task<bool> DeleteAsync(Guid id);
    }
}
