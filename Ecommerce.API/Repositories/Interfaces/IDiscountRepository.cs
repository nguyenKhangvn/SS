namespace Ecommerce.API.Repositories.Interfaces
{
    public interface IDiscountRepository
    {
        Task<IEnumerable<Discount>> GetAllAsync();
        Task<Discount?> GetByIdAsync(Guid id);
        Task<Discount> AddAsync(Discount discount);
        Task<bool> UpdateAsync(Discount discount);
        Task<bool> DeleteAsync(Guid id);
    }
}
