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
            order.OrderCode = $"ORD-{DateTime.UtcNow.Ticks}";
            order.Status = OrderStatus.PENDING;

            PrepareOrderItems(order);
            CalculateTotals(order);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> UpdateAsync(Order order)
        {
            PrepareOrderItems(order);
            CalculateTotals(order);

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

        private void PrepareOrderItems(Order order)
        {
            if (order.OrderItems == null) return;

            foreach (var item in order.OrderItems)
            {
                if (item.Id == Guid.Empty)
                    item.Id = Guid.NewGuid();

                item.TotalItemPrice = item.PriceAtOrder * item.Quantity;
            }
        }

        private void CalculateTotals(Order order)
        {
            order.SubTotal = order.OrderItems?.Sum(i => i.TotalItemPrice) ?? 0;
            order.TotalAmount = order.SubTotal + order.ShippingCost - order.DiscountAmount;
        }
    }
}
