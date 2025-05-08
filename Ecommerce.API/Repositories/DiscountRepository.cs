
using Microsoft.EntityFrameworkCore;
namespace Ecommerce.API.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly EcommerceDbContext _context;
        public DiscountRepository(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Discount>> GetAllAsync()
            => await _context.Discounts.Include(d => d.Products).ToListAsync();

        public async Task<Discount?> GetByIdAsync(Guid id)
            => await _context.Discounts.Include(d => d.Products).FirstOrDefaultAsync(d => d.Id == id);

        public async Task<Discount> AddAsync(Discount entity)
        {
            await _context.Discounts.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }


        public async Task<bool> UpdateAsync(Discount discount)
        {
            _context.Discounts.Update(discount);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var discount = await _context.Discounts.FindAsync(id);
            if (discount != null)
            {
                _context.Discounts.Remove(discount);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

    }
}
