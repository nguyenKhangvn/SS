using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Repositories.Interfaces
{
    public interface IProductRepository
    {
            Task<IEnumerable<Product>> GetAllAsync(string? include = null);
            Task<Product?> GetByIdAsync(Guid id, string? include = null);
            Task<Product> AddAsync(Product product);
            Task<Product?> UpdateAsync(Guid id, Product product);
            Task<bool> DeleteAsync(Guid id);
            Task<Product?> GetBySlugAsync(string slug);
            Task<PaginationResponse<Product>> GetAllProductsPaginatedAsync(
                ProductQueryParameters parameters,
                CancellationToken cancellationToken = default
            );
           Task<List<Product>> GetMostClickedProductsAsync(int topN, string? include = null);
           Task IncrementClickCountAsync(Guid productId);
    }
}
