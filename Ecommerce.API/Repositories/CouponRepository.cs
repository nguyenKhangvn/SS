
namespace Ecommerce.API.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly EcommerceDbContext _context;

        public CouponRepository(EcommerceDbContext context)
        {
            _context = context;
        }
        public async Task<Coupon> AddAsync(Coupon coupon)
        {
            await _context.Coupons.AddAsync(coupon);
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
            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Coupon>> GetAllAsync()
        {
            return await _context.Coupons.ToListAsync();
        }

        public async Task<Coupon?> GetByIdAsync(Guid id)
        {
           return await _context.Coupons.FindAsync(id);
        }

        public async Task<Coupon> UpdateAsync(Coupon coupon)
        {
            _context.Coupons.Update(coupon);
            await _context.SaveChangesAsync();
            return coupon;
        }
    }
}
