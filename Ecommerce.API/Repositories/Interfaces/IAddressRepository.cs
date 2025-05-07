namespace Ecommerce.API.Repositories
{
    public interface IAddressRepository
    {
        Task<IEnumerable<Address>> GetAllAsync();
        Task<Address?> GetByIdAsync(Guid id);
        Task<Address> CreateAsync(Address address);
        Task<Address> UpdateAsync(Address address);
        Task<bool> DeleteAsync(Guid id);
        Task<Address> SetDefaultAddress(Address address);
    }
}
