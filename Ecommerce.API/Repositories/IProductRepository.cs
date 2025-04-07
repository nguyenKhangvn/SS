
using System.Linq.Expressions;

namespace Ecommerce.API.Repositories
{
    public interface IProductRepository
    {
        Task<PaginationResponse<Product>> GetProductsAsync(
             PaginationRequest pagination,
             Expression<Func<Product, bool>>? filter = null, // Biểu thức lọc LINQ
             Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null, // Hàm sắp xếp LINQ
             string? includeProperties = null // Chuỗi tên các navigation property cần include (vd: "Category,Manufacturer")
         );

        IEnumerable<Product> GetProductsAsync();

        Task<Product?> GetProductByIdAsync(Guid productId, string? includeProperties = null);
        void AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(Guid productId);


    }
}
