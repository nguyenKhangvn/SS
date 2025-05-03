namespace Ecommerce.API.Repositories
{
    public interface ICouponRepository
    {
        Task<IEnumerable<Coupon>> GetAllAsync(bool onlyActive = true, Guid? userId = null);
        Task<Coupon?> GetByIdAsync(Guid id);
        Task<Coupon> AddAsync(Coupon coupon);
        Task<Coupon?> UpdateAsync(Coupon coupon);
        Task<bool> DeleteAsync(Guid id);
        Task<Coupon?> GetByCodeAsync(string code);
        Task<UserCoupon> SaveCouponAsync(UserCoupon userCoupon);
        Task<IEnumerable<Coupon>> GetSavedCouponsAsync(Guid userId);
    }
}