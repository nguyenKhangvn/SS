

namespace Ecommerce.API.Repositories
{
    public interface IProductRepository
    {
            Task<IEnumerable<Product>> GetAllAsync(string? include = null);
            Task<Product?> GetByIdAsync(Guid id, string? include = null);
            Task<Product> AddAsync(Product product);
            Task<Product?> UpdateAsync(Guid id, Product product);
            Task<bool> DeleteAsync(Guid id);
        
    }
}
