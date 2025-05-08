
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

        public async Task AddAsync(Discount discount)
        {
            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Discount discount)
        {
            _context.Discounts.Update(discount);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Discount discount)
        {
            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();
        }
    }
}
