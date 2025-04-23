
using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Repositories
{
    public class ProductStoreInventoryRepository : IProductStoreInventoryRepository
    {
        private  readonly EcommerceDbContext _context;
        public ProductStoreInventoryRepository(EcommerceDbContext context)
        {
            _context = context;
        }
        public async Task<ProductStoreInventory?> GetByIdAsync(Guid id)
        {
            return await _context.ProductStoreInventories
                .Include(psi => psi.Product)
                .Include(psi => psi.StoreLocation)
                .FirstOrDefaultAsync(psi => psi.Id == id);
        }

        public async Task<IEnumerable<ProductStoreInventory>> GetByProductIdAsync(Guid productId)
        {
            return await _context.ProductStoreInventories
                .Include(psi => psi.StoreLocation)
                .Where(psi => psi.ProductId == productId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductStoreInventory>> GetByStoreIdAsync(Guid storeId)
        {
            return await _context.ProductStoreInventories
                .Include(psi => psi.Product)
                .Include(p => p.StoreLocation)
                .Where(psi => psi.StoreLocationId == storeId)
                .ToListAsync();
        }

        public async Task<ProductStoreInventory?> GetByProductAndStoreAsync(Guid productId, Guid storeId)
        {
            return await _context.ProductStoreInventories
                .Include (psi => psi.Product)
                .Include(psi => psi.StoreLocation)
                .FirstOrDefaultAsync(psi => psi.ProductId == productId && psi.StoreLocationId == storeId);
        }

        public async Task AddAsync(ProductStoreInventory inventory)
        {
            await _context.ProductStoreInventories.AddAsync(inventory);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductStoreInventory inventory)
        {
            _context.ProductStoreInventories.Update(inventory);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductStoreInventory inventory)
        {
            _context.ProductStoreInventories.Remove(inventory);
            await _context.SaveChangesAsync();
        }
    }
}
