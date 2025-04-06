using Ecommerce.API.Dtos;

namespace Ecommerce.API.Interfaces
{
    public interface IProductService
    {
        // Updated to use ProductQueryParameters
        Task<PaginationResponse<ProductDto>> GetProductsAsync(Dtos.ProductQueryParameters queryParameters);
        Task<ProductDto?> GetProductByIdAsync(Guid productId);
        // Kept specific methods for clarity, though GetProductsAsync could handle them with filters
        Task<PaginationResponse<ProductDto>> GetProductsByCategoryAsync(Guid categoryId, Dtos.ProductQueryParameters queryParameters);
        Task<PaginationResponse<ProductDto>> GetProductsByManufacturerAsync(Guid manufacturerId, Dtos.ProductQueryParameters queryParameters);
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
        Task<bool> UpdateProductAsync(Guid productId, UpdateProductDto updateProductDto);
        Task<bool> DeleteProductAsync(Guid productId);
        // Optional: Task<bool> UpdateProductStatusAsync(Guid productId, bool isActive);
    }
}
