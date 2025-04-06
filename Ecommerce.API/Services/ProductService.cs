
using System.Linq.Expressions;

namespace Ecommerce.API.Services
{
    public class ProductService : IProductRepository
    {
        private Dictionary<Guid, Product> _products = [];
        private EcommerceServices _service;
        public Task AddProductAsync(Product product)
        {
            _service.DbContext.Products.Add(new Product
            {
                Name = product.Name,
                Description = product.Description,
            });

            _service.DbContext.SaveChanges();
            return Task.FromResult(new Product
            {
                Name = product.Name,
                Description = product.Description,
            });
        }

       
        public Task<bool> DeleteProductAsync(Guid productId)
        {
            throw new NotImplementedException();
        }

     

        public Task<Product?> GetProductByIdAsync(Guid productId, string? includeProperties = null)
        {
            throw new NotImplementedException();
        }

      

        public Task<PaginationResponse<Product>> GetProductsAsync(PaginationRequest pagination, Expression<Func<Product, bool>>? filter = null, Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null, string? includeProperties = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Product> GetProductsAsync()
        {
            return _products.Values;
        }

        public Task<bool> ProductExistsAsync(Guid productId)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

       
        public Task UpdateProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        void IProductRepository.AddProductAsync(Product product)
        {
            throw new NotImplementedException();
        }

        Task IProductRepository.DeleteProductAsync(Guid productId)
        {
            return DeleteProductAsync(productId);
        }
    }
}
