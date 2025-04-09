namespace Ecommerce.API.Repositories
{
    public interface IOrderItemRepository
    {
        Task<IEnumerable<OrderItem>> GetAllAsync();
        Task<OrderItem?> GetByIdAsync(Guid id);
        Task<OrderItem> CreateAsync(OrderItem orderItem);
        Task<OrderItem?> UpdateAsync(Guid id, OrderItem orderItem);
        Task<bool> DeleteAsync(Guid id);
    }
}
