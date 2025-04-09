using Ecommerce.Infrastructure.Dtos;

namespace Ecommerce.API.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductAsync(string? includeProperties = null);
        Task<ProductDto?> GetProductByIdAsync(Guid id, string? includeProperties = null);
        Task<ProductDto> AddProductAsync(ProductCreateDto dto);
        Task<ProductDto?> UpdateProductAsync(Guid id, ProductUpdateDto dto);
        Task<bool> DeleteProductAsync(Guid id);

    }
}
