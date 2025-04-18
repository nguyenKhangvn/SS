﻿using Ecommerce.API.Services.Interfaces;
using Ecommerce.Infrastructure.Dtos;
using Ecommerce.Infrastructure.Models.Dtos;
using System.Net.WebSockets;
using ProductQueryParameters = Ecommerce.Infrastructure.Models.ProductQueryParameters;

namespace Ecommerce.API.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        public ProductService(IProductRepository productRepository, IImageService imageService, IMapper mapper)
        {
            _productRepository = productRepository;
            _imageService = imageService;
            _mapper = mapper;
        }

        public async Task<ProductDto> AddProductAsync([FromForm] ProductCreateDto dto)
        {
            var product = _mapper.Map<Product>(dto);

            var added = await _productRepository.AddAsync(product);
            if (dto.ImageFile1 != null)
            {
                var imageUrl = await _imageService.UpdateImageAsync(dto.ImageFile1);
                var image = new ImageDto
                {
                    ProductId = added.Id,
                    Url = imageUrl,
                    AltText = $"Ảnh minh họa {added.Name}",
                    DisplayOrder = 0
                };
                await _imageService.AddImageAsync(image);
            }
            if (dto.ImageFile2 != null)
            {
                var imageUrl = await _imageService.UpdateImageAsync(dto.ImageFile2);
                var image = new ImageDto
                {
                    ProductId = added.Id,
                    Url = imageUrl,
                    AltText = $"Ảnh minh họa {added.Name}",
                    DisplayOrder = 0
                };
                await _imageService.AddImageAsync(image);
            }
            if (dto.ImageFile3 != null)
            {
                var imageUrl = await _imageService.UpdateImageAsync(dto.ImageFile3);
                var image = new ImageDto
                {
                    ProductId = added.Id,
                    Url = imageUrl,
                    AltText = $"Ảnh minh họa {added.Name}",
                    DisplayOrder = 0
                };
                await _imageService.AddImageAsync(image);
            }
            if (dto.ImageFile4 != null)
            {
                var imageUrl = await _imageService.UpdateImageAsync(dto.ImageFile4);
                var image = new ImageDto
                {
                    ProductId = added.Id,
                    Url = imageUrl,
                    AltText = $"Ảnh minh họa {added.Name}",
                    DisplayOrder = 0
                };
                await _imageService.AddImageAsync(image);
            }

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

        public async Task<PaginationResponse<ProductDto>> GetAllProductsPaginatedAsync(
            ProductQueryParameters parameters,
            CancellationToken cancellationToken = default)
        {
            var paginatedResponse = await _productRepository.GetAllProductsPaginatedAsync(
                parameters,
                cancellationToken
            );

            var itemDtos = _mapper.Map<IEnumerable<ProductDto>>(paginatedResponse.Items);

            return new PaginationResponse<ProductDto>(
                paginatedResponse.PageIndex,
                paginatedResponse.PageSize,
                paginatedResponse.TotalCount,
                itemDtos
            );
        }

    }
}
