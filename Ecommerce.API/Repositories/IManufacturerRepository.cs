namespace Ecommerce.API.Repositories
{
    public interface IManufacturerRepository
    {
        Task<IEnumerable<Manufacturer>> GetAllAsync();
        Task<Manufacturer?> GetByIdAsync(Guid id);
        Task<Manufacturer> CreateAsync(Manufacturer manufacturer);
        Task<Manufacturer> UpdateAsync(Manufacturer manufacturer);
        Task<bool> DeleteAsync(Guid id);
    }
}
