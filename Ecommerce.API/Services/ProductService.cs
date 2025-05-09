using Ecommerce.API.Services.Interfaces;
using Ecommerce.Infrastructure.Dtos;
using Ecommerce.Infrastructure.Entity;
using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Polly.CircuitBreaker;
using System.Globalization;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using static Ecommerce.Infrastructure.Models.Dtos.ProductCreateDto;
using ProductQueryParameters = Ecommerce.Infrastructure.Models.ProductQueryParameters;

namespace Ecommerce.API.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IImageService _imageService;
        private readonly IProductStoreInventoryService _productStoreInventoryService;
        private readonly IMapper _mapper;
        private readonly EcommerceDbContext _context;
        public ProductService(
            IProductRepository productRepository, 
            IImageService imageService,
            IProductStoreInventoryService productStoreInventoryService,
            IMapper mapper,
            EcommerceDbContext context
        )
        {
            _productRepository = productRepository;
            _imageService = imageService;
            _productStoreInventoryService = productStoreInventoryService;
            _mapper = mapper;
            _context = context;
        }

        public async Task<ProductDto> AddProductAsync([FromForm] ProductCreateDto dto)
        {
            dto.Slug = GenerateSlug(dto.Name);
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
            // add quantity
            if (dto.Quantity != null && dto.StoreId != null)
            {
                var inventoryDto = new AddOrUpdateProductStoreInventoryDto
                {
                    ProductId = added.Id, 
                    StoreLocationId = dto.StoreId,
                    Quantity = dto.Quantity
                };
                await _productStoreInventoryService.AddAsync(inventoryDto);
            }

            return _mapper.Map<ProductDto>(product);
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


        public async Task<ProductDto?> UpdateProductAsync(Guid id, [FromForm] ProductCreateDto dto)
        {
            // 1. Lấy sản phẩm hiện tại từ database
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                return null;
            }

            // 2. Cập nhật thông tin cơ bản
            dto.Slug = GenerateSlug(dto.Name);
            _mapper.Map(dto, existingProduct); // Map từ dto vào existingProduct

            // 3. Cập nhật sản phẩm trong database
            var updatedProduct = await _productRepository.UpdateAsync(id, existingProduct);
            if (updatedProduct == null)
            {
                return null;
            }

            // 4. Xử lý ảnh
            await ProcessProductImages(dto, updatedProduct.Id, updatedProduct.Name);

            // 5. Cập nhật inventory nếu có thay đổi
            await UpdateInventoryIfChanged(updatedProduct.Id, dto.StoreId, dto.Quantity);

            // 6. Trả về sản phẩm đã cập nhật
            return await GetFullProductDto(updatedProduct.Id);
        }

        private async Task ProcessProductImages(ProductCreateDto dto, Guid productId, string productName)
        {
            // Xử lý ảnh mới
            var imageFiles = new[] { dto.ImageFile1, dto.ImageFile2, dto.ImageFile3, dto.ImageFile4 }
                .Where(f => f != null)
                .ToList();

            foreach (var imageFile in imageFiles)
            {
                var imageUrl = await _imageService.UpdateImageAsync(imageFile);
                var image = new ImageDto
                {
                    ProductId = productId,
                    Url = imageUrl,
                    AltText = $"Ảnh minh họa {productName}",
                    DisplayOrder = 0
                };
                await _imageService.AddImageAsync(image);
            }

            // Xử lý ảnh bị xóa (nếu có)
            //if (dto.DeletedImageIds != null && dto.DeletedImageIds.Any())
            //{
            //    foreach (var imageId in dto.DeletedImageIds)
            //    {
            //        await _imageService.DeleteImageAsync(imageId);
            //    }
            //}
        }
        //cloud img
        public async Task<ProductDto?> UpdateProductAsyncToCloud(Guid id, [FromForm] ProductCreateDto dto)
        {
            // 1. Lấy sản phẩm hiện tại từ database
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                return null;
            }

            // 2. Cập nhật thông tin cơ bản
            dto.Slug = GenerateSlug(dto.Name);
            _mapper.Map(dto, existingProduct); // Map từ dto vào existingProduct

            // 3. Cập nhật sản phẩm trong database
            var updatedProduct = await _productRepository.UpdateAsync(id, existingProduct);
            if (updatedProduct == null)
            {
                return null;
            }

            // 4. Xử lý ảnh
            await ProcessProductImagesToCloud(dto, updatedProduct.Id, updatedProduct.Name);

            // 5. Cập nhật inventory nếu có thay đổi
            await UpdateInventoryIfChanged(updatedProduct.Id, dto.StoreId, dto.Quantity);

            // 6. Trả về sản phẩm đã cập nhật
            return await GetFullProductDto(updatedProduct.Id);
        }
        private async Task ProcessProductImagesToCloud(ProductCreateDto dto, Guid productId, string productName)
        {
            // Xử lý ảnh mới
            var imageFiles = new[] { dto.ImageFile1, dto.ImageFile2, dto.ImageFile3, dto.ImageFile4 }
                .Where(f => f != null)
                .ToList();

            foreach (var imageFile in imageFiles)
            {
                var imageUrl = await _imageService.UpdateImageToCloudAsync(imageFile);
                var image = new ImageDto
                {
                    ProductId = productId,
                    Url = imageUrl,
                    AltText = $"Ảnh minh họa {productName}",
                    DisplayOrder = 0
                };
                await _imageService.AddImageAsync(image);
            }

            // Xử lý ảnh bị xóa (nếu có)
            //if (dto.DeletedImageIds != null && dto.DeletedImageIds.Any())
            //{
            //    foreach (var imageId in dto.DeletedImageIds)
            //    {
            //        await _imageService.DeleteImageAsync(imageId);
            //    }
            //}
        }


        private async Task UpdateInventoryIfChanged(Guid productId, Guid? storeId, int? quantity)
        {
            if (storeId == null || quantity == null) return;

            var existingInventory = await _productStoreInventoryService.GetByProductAndStoreAsync(productId, storeId.Value);

            if (existingInventory == null)
            {
                // Nếu chưa có inventory, tạo mới
                var inventoryDto = new AddOrUpdateProductStoreInventoryDto
                {
                    ProductId = productId,
                    StoreLocationId = storeId.Value,
                    Quantity = quantity.Value
                };
                await _productStoreInventoryService.AddAsync(inventoryDto);
            }
            else if (existingInventory.Quantity != quantity.Value)
            {
                // Nếu có inventory và số lượng thay đổi, cập nhật
                await _productStoreInventoryService.UpdateQuantityAsync(productId, storeId.Value, quantity.Value);
            }
        }

        private async Task<ProductDto> GetFullProductDto(Guid productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            var productDto = _mapper.Map<ProductDto>(product);

            // Lấy danh sách ảnh hiện tại của sản phẩm
            productDto.Images = await _imageService.GetImagesByProductIdAsync(productId);
            return productDto;
        }


        private static string GenerateSlug(string name)
        {
            name = name.Replace("đ", "d").Replace("Đ", "D");

            string normalized = name.ToLowerInvariant().Normalize(System.Text.NormalizationForm.FormD);
            var slug = new string(normalized
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray());

            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-");
            slug = Regex.Replace(slug, @"-+", "-");

            return slug.Trim('-');
        }

        public async Task<ProductDto?> GetProductBySlugAsync(string slug)
        {
            var product = await _productRepository.GetBySlugAsync(slug);
            return product == null ? null : _mapper.Map<ProductDto>(product);
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
                itemDtos.ToList()
            );
        }
        //add product to cloud
        public async Task<ProductDto> AddProductAsyncToCloud([FromForm] ProductCreateDto dto)
        {
            dto.Slug = GenerateSlug(dto.Name);
            var product = _mapper.Map<Product>(dto);

            var added = await _productRepository.AddAsync(product);
            if (dto.ImageFile1 != null)
            {
                var imageUrl = await _imageService.UpdateImageToCloudAsync(dto.ImageFile1);
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
                var imageUrl = await _imageService.UpdateImageToCloudAsync(dto.ImageFile2);
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
                var imageUrl = await _imageService.UpdateImageToCloudAsync(dto.ImageFile3);
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
                var imageUrl = await _imageService.UpdateImageToCloudAsync(dto.ImageFile4);
                var image = new ImageDto
                {
                    ProductId = added.Id,
                    Url = imageUrl,
                    AltText = $"Ảnh minh họa {added.Name}",
                    DisplayOrder = 0
                };
                await _imageService.AddImageAsync(image);
            }
            // add quantity
            if (dto.Quantity != null && dto.StoreId != null)
            {
                var inventoryDto = new AddOrUpdateProductStoreInventoryDto
                {
                    ProductId = added.Id,
                    StoreLocationId = dto.StoreId,
                    Quantity = dto.Quantity
                };
                await _productStoreInventoryService.AddAsync(inventoryDto);
            }

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto?> BuyProduct(Guid productId, UpdateAProduct dto)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return null;
            }

            // Find the specific inventory entry for the product in the desired store  
            var storeInventory = product.StoreInventories.FirstOrDefault();
            if (storeInventory == null)
            {
                throw new InvalidOperationException("No inventory found for the product in the store.");
            }

            // Update the quantity  
            await _productRepository.ExecuteInTransactionAsync(async () =>
            {
                storeInventory.Quantity -= dto.Quantity;
                await Task.CompletedTask;
            });
            await _productRepository.UpdateAsync(productId, product);
            return _mapper.Map<ProductDto>(product);
        }
    }
}
