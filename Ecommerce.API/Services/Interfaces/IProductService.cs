using Ecommerce.Infrastructure.Dtos;
using Ecommerce.Infrastructure.Models.Dtos;
using ProductQueryParameters = Ecommerce.Infrastructure.Models.ProductQueryParameters;

namespace Ecommerce.API.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductAsync(string? includeProperties = null);
        Task<ProductDto?> GetProductByIdAsync(Guid id, string? includeProperties = null);
        Task<ProductDto> AddProductAsync(ProductCreateDto dto);
        Task<ProductDto> AddProductAsyncToCloud(ProductCreateDto dto);
        Task<ProductDto?> UpdateProductAsync(Guid id, ProductCreateDto dto);
        Task<ProductDto?> UpdateProductAsyncToCloud(Guid id, ProductCreateDto dto);

        Task<bool> DeleteProductAsync(Guid id);
        Task<ProductDto?> GetProductBySlugAsync(string slug);
        Task<PaginationResponse<ProductDto>> GetAllProductsPaginatedAsync(
           ProductQueryParameters parameters,
            CancellationToken cancellationToken = default
       );
        //goi y sp
        Task<List<ProductDto>> GetRecommendedProductsAsync(int topN);
        Task IncrementClickCountAsync(Guid productId);
    }
}
