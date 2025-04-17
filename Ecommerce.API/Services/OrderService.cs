using Ecommerce.Infrastructure.Entity;
using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IProductService _product;
        private readonly IOrderItemRepository _orderItemRepository;

        public OrderService(IOrderRepository orderRepository, IProductService productService, IOrderItemRepository orderItemRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _product = productService;
            _orderItemRepository = orderItemRepository;
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
        //private async Task<decimal> CalculateOrderTotalAsync(List<OrderItemDto> orderItems)
        //{
        //    decimal total = 0;
        //    foreach (var item in orderItems)
        //    {
        //        var product = await _product.GetProductByIdAsync(item.ProductId);
        //        if (product != null)
        //        {
        //            total += product.Price * item.Quantity;
        //        }
        //    }
        //    return total;
        //}

        public async Task<OrderDto> CreateAsync(OrderDto dto)
        {
            //dto.TotalAmount = await CalculateOrderTotalAsync(dto.OrderItems);

            //var orderEntity = _mapper.Map<Order>(dto);
            //orderEntity.OrderCode = $"ORD-{DateTime.UtcNow.Ticks}";
            //orderEntity.Status = OrderStatus.PENDING;
            //var createdOrder = await _orderRepository.CreateAsync(orderEntity);
            //return _mapper.Map<OrderDto>(createdOrder);


            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();
            foreach (var itemDto in dto.OrderItems)
            {
                // Lấy thông tin sản phẩm
                var product = await _product.GetProductByIdAsync(itemDto.ProductId);
                if (product == null)
                {
                    throw new Exception($"Product {itemDto.ProductId} not found");
                }

                // Tạo OrderItem
                var orderItem = new OrderItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    PriceAtOrder = product.Price, // Lưu giá tại thời điểm đặt hàng
                    TotalItemPrice = product.Price * itemDto.Quantity
                };

                orderItems.Add(orderItem);
                totalAmount += orderItem.TotalItemPrice;
            }

            var orderEntity = new Order
            {
                OrderCode = $"ORD-{DateTime.UtcNow.Ticks}",
                UserId = dto.UserId,
                ShippingAddressId = dto.ShippingAddressId,
                BillingAddressId = dto.BillingAddressId,
                CouponId = dto.CouponId,
                PaymentId = dto.PaymentId,
                Notes = dto.Notes,
                TotalAmount = totalAmount,
                Status = OrderStatus.PENDING,
                OrderItems = orderItems 
            };
            var createdOrder = await _orderRepository.CreateAsync(orderEntity);
            
            return _mapper.Map<OrderDto>(orderEntity);
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

        public async Task<OrderDto?> UpdateStatusAsync(Guid id, string Status)
        {
            var updatedOrder = await _orderRepository.UpdateStatusAsync(id, Status);
            return updatedOrder == null ? null : _mapper.Map<OrderDto>(updatedOrder);
        }
    }


}
