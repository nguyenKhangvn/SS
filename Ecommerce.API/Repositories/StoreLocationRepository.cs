
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Repositories
{
    public class StoreLocationRepository : IStoreLocationRepository
    {
        private EcommerceDbContext _context;

        public StoreLocationRepository(EcommerceDbContext context)
        {
            _context = context;
        }
        public async Task<StoreLocation> AddAsync(StoreLocation storeLocation)
        {
            await _context.StoreLocations.AddAsync(storeLocation);
            await _context.SaveChangesAsync();
            return storeLocation;

        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var storeLocation = await _context.StoreLocations.FindAsync(id);
            if (storeLocation == null)
            {
                return false;
            }

            _context.StoreLocations.Remove(storeLocation);
            await _context.SaveChangesAsync();
            return true;

        }

        public IQueryable<StoreLocation> GetAll()
        {
            return _context.StoreLocations.AsQueryable();
        }

        public async Task<StoreLocation?> GetByIdAsync(Guid id, string? includeProperties = null)
        {
            var query = _context.StoreLocations.AsQueryable();

            if (!string.IsNullOrEmpty(includeProperties))
            {
                // Thêm các thuộc tính liên kết vào query (Eager Loading)
                query = query.Include(includeProperties);
            }

            return await query.FirstOrDefaultAsync(sl => sl.Id == id);

        }

        public async Task<StoreLocation> UpdateAsync(StoreLocation storeLocation)
        {
            _context.StoreLocations.Update(storeLocation);
            await _context.SaveChangesAsync();
            return storeLocation;
        }
    }
}
