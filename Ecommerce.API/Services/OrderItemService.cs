using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services
{
    public class OrderItemService: IOrderItemService
    {
        private readonly IOrderItemRepository _repository;
        private readonly IMapper _mapper;

        public OrderItemService(IOrderItemRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<OrderItemDto> CreateAsync(OrderItemDto dto)
        {
            var orderItem = _mapper.Map<OrderItem>(dto);
            var createdOrderItem = await _repository.CreateAsync(orderItem);
            return _mapper.Map<OrderItemDto>(createdOrderItem);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<OrderItemDto>> GetAllAsync()
        {
            var orderItems = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<OrderItemDto>>(orderItems);
        }

        public async Task<OrderItemDto?> GetByIdAsync(Guid id)
        {
            var orderItem = await _repository.GetByIdAsync(id);
            if (orderItem == null)
            {
                return null;
            }
            return _mapper.Map<OrderItemDto>(orderItem);
        }

        public async Task<OrderItemDto?> UpdateAsync(Guid id, OrderItemDto dto)
        {
            var orderItem = _mapper.Map<OrderItem>(dto);
            var updatedOrderItem = await _repository.UpdateAsync(id, orderItem);
            return _mapper.Map<OrderItemDto>(updatedOrderItem);
        }
    }
}
