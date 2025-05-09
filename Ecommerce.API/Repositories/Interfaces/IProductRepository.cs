using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;

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
            Task ExecuteInTransactionAsync(Func<Task> operation);
    }
}
