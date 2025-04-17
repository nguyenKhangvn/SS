using Ecommerce.API.Services.Interfaces;
using Ecommerce.Infrastructure.Entity;
using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services
{
    public class OrderItemService: IOrderItemService
    {
        private readonly IOrderItemRepository _repoOrderItem;
        private readonly IMapper _mapper;
        private readonly IOrderRepository _repoOrder;
        private readonly IProductService _product;

        public OrderItemService(IOrderItemRepository repository, IOrderRepository repoOrder, IProductService productService, IMapper mapper)
        {
            _repoOrderItem = repository;
            _mapper = mapper;
            _repoOrder = repoOrder;
            _product = productService;
        }

        public async Task<OrderItemDto> CreateAsync(OrderItemDto dto)
        {
            var order = await _repoOrder.GetByIdAsync(dto.OrderId);
            if (order == null)
            {
                throw new Exception("Order not found");
            }

            var product = await _product.GetProductByIdAsync(dto.ProductId);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            var orderItem = new OrderItem
            {
                OrderId = dto.OrderId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                PriceAtOrder = product.Price,
                TotalItemPrice = dto.Quantity * product.Price

            };
            order.TotalAmount += orderItem.TotalItemPrice;
            await _repoOrder.UpdateAsync(order);

            var createdOrderItem = await _repoOrderItem.CreateAsync(orderItem);
            return _mapper.Map<OrderItemDto>(createdOrderItem);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _repoOrderItem.DeleteAsync(id);
        }

        public async Task<IEnumerable<OrderItemDto>> GetAllAsync()
        {
            var orderItems = await _repoOrderItem.GetAllAsync();
            return _mapper.Map<IEnumerable<OrderItemDto>>(orderItems);
        }

        public async Task<OrderItemDto?> GetByIdAsync(Guid id)
        {
            var orderItem = await _repoOrderItem.GetByIdAsync(id);
            if (orderItem == null)
            {
                return null;
            }
            return _mapper.Map<OrderItemDto>(orderItem);
        }

        public async Task<OrderItemDto?> UpdateAsync(Guid id, OrderItemDto dto)
        {
            var orderItem = _mapper.Map<OrderItem>(dto);
            var updatedOrderItem = await _repoOrderItem.UpdateAsync(id, orderItem);
            return _mapper.Map<OrderItemDto>(updatedOrderItem);
        }
    }
}
