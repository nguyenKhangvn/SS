using Ecommerce.Infrastructure.Entity;
using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return orders.Select(_mapper.Map<OrderDto>).ToList();
        }

        public async Task<OrderDto?> GetByIdAsync(Guid id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            await _orderRepository.GetByIdAsync(id);
            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> CreateAsync(OrderDto dto)
        {
            var orderEntity = _mapper.Map<Order>(dto);
            var createdOrder = await _orderRepository.CreateAsync(orderEntity);
            return _mapper.Map<OrderDto>(createdOrder);
        }

        public async Task<OrderDto> UpdateAsync(OrderDto dto)
        {
            var orderEntity =  _mapper.Map<Order>(dto);
            var updateOrder = await _orderRepository.UpdateAsync(orderEntity);
            return _mapper.Map<OrderDto>(updateOrder);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
           return await _orderRepository.DeleteAsync(id);
        }

       
    }


}
