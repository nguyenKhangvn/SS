using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services
{
    public class ProductStoreInventoryService : IProductStoreInventoryService
    {
        private readonly IProductStoreInventoryRepository _repository;
        private readonly IMapper _mapper;

        public ProductStoreInventoryService(
            IProductStoreInventoryRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ProductStoreInventoryDto> GetByIdAsync(Guid id)
        {
            var inventory = await _repository.GetByIdAsync(id);
            return _mapper.Map<ProductStoreInventoryDto>(inventory);
        }

        public async Task<IEnumerable<ProductStoreInventoryDto>> GetByProductIdAsync(Guid productId)
        {
            var inventories = await _repository.GetByProductIdAsync(productId);
            return _mapper.Map<IEnumerable<ProductStoreInventoryDto>>(inventories);
        }

        public async Task<IEnumerable<ProductStoreInventoryDto>> GetByStoreIdAsync(Guid storeId)
        {
            var inventories = await _repository.GetByStoreIdAsync(storeId);
            return inventories.Select(p => new ProductStoreInventoryDto
            {
                Id = p.Id,
                ProductId = p.ProductId,
                ProductName = p.Product?.Name,
                StoreLocationId = p.StoreLocationId,
                StoreName = p.StoreLocation?.Name,
                Quantity = p.Quantity,
                Sold = p.Sold
            }).ToList();
         //   return _mapper.Map<IEnumerable<ProductStoreInventoryDto>>(inventories);
        }

        public async Task<ProductStoreInventoryDto?> GetByProductAndStoreAsync(Guid productId, Guid storeId)
        {
            var inventory = await _repository.GetByProductAndStoreAsync(productId, storeId);
            return inventory == null ? null : _mapper.Map<ProductStoreInventoryDto>(inventory);
        }

        public async Task<ProductStoreInventoryDto> AddAsync(AddOrUpdateProductStoreInventoryDto dto)
        {
            var existing = await _repository.GetByProductAndStoreAsync(dto.ProductId, dto.StoreLocationId);
            var inventory = _mapper.Map<ProductStoreInventory>(dto);
            await _repository.AddAsync(inventory);
            return _mapper.Map<ProductStoreInventoryDto>(inventory);
        }

        public async Task<ProductStoreInventoryDto> UpdateAsync(Guid id, AddOrUpdateProductStoreInventoryDto dto)
        {
            var inventory = await _repository.GetByIdAsync(id);
            _mapper.Map(dto, inventory);
            _repository.UpdateAsync(inventory);
            return _mapper.Map<ProductStoreInventoryDto>(inventory);
        }

        public async Task UpdateQuantityAsync(Guid productId, Guid storeId, int quantityChange)
        {
            var inventory = await _repository.GetByProductAndStoreAsync(productId, storeId);

            if (inventory == null)
            {
                inventory = new ProductStoreInventory
                {
                    ProductId = productId,
                    StoreLocationId = storeId,
                    Quantity = quantityChange,
                };
                await _repository.AddAsync(inventory);
            }
            else
            {
                inventory.Quantity += quantityChange;
                //inventory.Sold -= quantityChange;
            }

        }

        public async Task DeleteAsync(Guid id)
        {
            var inventory = await _repository.GetByIdAsync(id);
            _repository.DeleteAsync(inventory);
            _mapper.Map<ProductStoreInventoryDto>(inventory);
        }
    }
}
