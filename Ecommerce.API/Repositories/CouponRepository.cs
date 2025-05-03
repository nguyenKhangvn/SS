namespace Ecommerce.API.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly EcommerceDbContext _context;

        public CouponRepository(EcommerceDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<IEnumerable<Coupon>> GetAllAsync(bool onlyActive = true, Guid? userId = null)
        {
            var query = _context.Coupons.AsQueryable();

            if (onlyActive)
            {
                query = query.Where(c => c.IsActive && (c.EndTime == null || c.EndTime >= DateTime.UtcNow));
            }

            if (userId.HasValue)
            {
                query = query.Where(c => c.UserId == null || c.UserId == userId);
            }

            return await query.ToListAsync();
        }

        public async Task<Coupon?> GetByIdAsync(Guid id)
        {
            return await _context.Coupons
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Coupon> AddAsync(Coupon coupon)
        {
            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();
            return coupon;
        }

        public async Task<Coupon?> UpdateAsync(Coupon coupon)
        {
            _context.Coupons.Update(coupon);
            await _context.SaveChangesAsync();
            return coupon;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
            {
                return false;
            }
            coupon.IsActive = false;
            _context.Coupons.Update(coupon);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Coupon?> GetByCodeAsync(string code)
        {
            return await _context.Coupons
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.Code == code);
        }

        public async Task<UserCoupon> SaveCouponAsync(UserCoupon userCoupon)
        {
            _context.UserCoupons.Add(userCoupon);
            await _context.SaveChangesAsync();
            return userCoupon;
        }

        public async Task<IEnumerable<Coupon>> GetSavedCouponsAsync(Guid userId)
        {
            return await _context.UserCoupons
                .Where(uc => uc.UserId == userId)
                .Include(uc => uc.Coupon)
                .Where(uc => uc.Coupon.IsActive && (uc.Coupon.EndTime == null || uc.Coupon.EndTime >= DateTime.UtcNow))
                .Select(uc => uc.Coupon)
                .ToListAsync();
        }
    }
}