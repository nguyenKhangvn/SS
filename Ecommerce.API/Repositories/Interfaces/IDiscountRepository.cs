namespace Ecommerce.API.Repositories.Interfaces
{
    public interface IDiscountRepository
    {
        Task<IEnumerable<Discount>> GetAllAsync();
        Task<Discount?> GetByIdAsync(Guid id);
        Task AddAsync(Discount discount);
        Task UpdateAsync(Discount discount);
        Task DeleteAsync(Discount discount);
    }
}
