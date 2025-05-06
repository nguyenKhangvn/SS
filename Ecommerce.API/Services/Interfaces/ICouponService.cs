using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services.Interfaces
{
    public interface ICouponService
    {
        Task<IEnumerable<CouponDto>> GetAllAsync(Guid? userId, bool onlyActive);
        Task<IEnumerable<CouponDto>> GetCouponsActiveAsync(bool onlyActive = true, Guid? userId = null);
        Task<CouponDto?> GetByIdAsync(Guid id);
        Task<CouponDto> AddAsync(CouponCreateDto dto);
        Task<CouponDto?> UpdateAsync(Guid id, CouponCreateDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<ApplyCouponResult> ApplyCouponAsync(string code, Guid? userId, decimal orderTotal);
        Task<SaveCouponResult> SaveCouponAsync(Guid userId, string couponCode);
        Task<IEnumerable<CouponDto>> GetSavedCouponsAsync(Guid userId);
    }
}