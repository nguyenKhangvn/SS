using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Repositories
{
    public class ManufacturerRepository : IManufacturerRepository
    {
        private readonly EcommerceDbContext _context;

        public ManufacturerRepository(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Manufacturer>> GetAllAsync()
        {
            return await _context.Manufacturers.ToListAsync();
        }

        public async Task<Manufacturer?> GetByIdAsync(Guid id)
        {
            return await _context.Manufacturers.FindAsync(id);
        }

        public async Task<Manufacturer> CreateAsync(Manufacturer manufacturer)
        {
            _context.Manufacturers.Add(manufacturer);
            await _context.SaveChangesAsync();
            return manufacturer;
        }

        public async Task<Manufacturer> UpdateAsync(Manufacturer manufacturer)
        {
            _context.Manufacturers.Update(manufacturer);
            await _context.SaveChangesAsync();
            return manufacturer;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var manufacturer = await _context.Manufacturers.FindAsync(id);
            if (manufacturer == null) return false;

            _context.Manufacturers.Remove(manufacturer);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
