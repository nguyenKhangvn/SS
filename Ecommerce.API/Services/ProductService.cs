using Ecommerce.Infrastructure.Dtos;
using Ecommerce.Infrastructure.Models.Dtos;
using System.Net.WebSockets;

namespace Ecommerce.API.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ProductDto> AddProductAsync(ProductCreateDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            var added = await _productRepository.AddAsync(product);
            return _mapper.Map<ProductDto>(added);
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            return await _productRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductAsync(string? includeProperties = null)
        {
            var query = await _productRepository.GetAllAsync(includeProperties);
            return _mapper.Map<IEnumerable<ProductDto>>(query);
        }

        public async Task<ProductDto?> GetProductByIdAsync(Guid id, string? includeProperties = null)
        {
            var product = await _productRepository.GetByIdAsync(id, includeProperties);
            if (product == null)
            {
                return null;
            }
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto?> UpdateProductAsync(Guid id, ProductUpdateDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            var updated = await _productRepository.UpdateAsync(id, product);
            if (updated == null)
            {
                return null;
            }
            return _mapper.Map<ProductDto>(updated);
        }
    }
}
