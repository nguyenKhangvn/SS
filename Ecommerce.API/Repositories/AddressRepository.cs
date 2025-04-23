

using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly EcommerceDbContext _context;
        public AddressRepository(EcommerceDbContext context)
        {
            _context = context;
        }
        public async Task<Address> CreateAsync(Address address)
        {
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null) return false;

            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<IEnumerable<Address>> GetAllAsync()
        {
            return await _context.Addresses.ToListAsync();
        }

        public async Task<Address?> GetByIdAsync(Guid id)
        {
            return await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Address> SetDefaultAddress(Address address)
        {
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async  Task<Address> UpdateAsync(Address address)
        {
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();
            return address;
        }
    }
}
