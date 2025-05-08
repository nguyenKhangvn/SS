using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly EcommerceDbContext _context;

        public OrderRepository(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> CreateAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<Order?> UpdateStatusAsync(Guid id, string Status)
        {
            var existingOrder = await _context.Orders.FindAsync(id);
            if (existingOrder == null)
                return null;

            if (!Enum.TryParse<OrderStatus>(Status, out var orderStatus))
            {
                throw new ArgumentException($"Invalid order status: {Status}");
            }

            existingOrder.Status = orderStatus;

            _context.Orders.Update(existingOrder);
            await _context.SaveChangesAsync();
            return existingOrder;
        }

        public async Task<IEnumerable<Order>> GetAllByUserId(Guid id)
        {
            var userOrders = await _context.Orders.Include(order => order.OrderItems)
                                       .Where(order => order.UserId == id)
                                       .OrderByDescending(order => order.CreatedAt)
                                       .ToListAsync();
            return userOrders;

        }

        public async Task<Order> GetOrderByOrderCode(string orderCode)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(ord => ord.OrderCode == orderCode);
            return order ?? throw new InvalidOperationException("Order not found.");
        }
    }
}
