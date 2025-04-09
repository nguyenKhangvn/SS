namespace Ecommerce.API.Repositories
{
    public class OrderItemRepository: IOrderItemRepository
    {
        private readonly EcommerceDbContext _context;
        public OrderItemRepository(EcommerceDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<OrderItem>> GetAllAsync()
        {
            return await _context.OrderItems
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                .ToListAsync();
        }
        public async Task<OrderItem?> GetByIdAsync(Guid id)
        {
            var query = _context.OrderItems.AsQueryable();
            return await query
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                .FirstOrDefaultAsync(oi => oi.Id == id);
        }
        public async Task<OrderItem> CreateAsync(OrderItem orderItem)
        {
            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();
            return orderItem;
        }
        public async Task<OrderItem?> UpdateAsync(Guid id, OrderItem orderItem)
        {
            var existingOrderItem = await _context.OrderItems.FindAsync(id);
            if (existingOrderItem == null)
            {
                return null;
            }
            _context.OrderItems.Update(orderItem);
            await _context.SaveChangesAsync();
            return orderItem;
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
            {
                return false;
            }
            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
