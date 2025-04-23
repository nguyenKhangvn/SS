using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;

        public AddressService(IAddressRepository addressRepository, IMapper mapper)
        {
            _addressRepository = addressRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AddressDto>> GetAllAsync()
        {
            var entities = await _addressRepository.GetAllAsync();
            return entities.Select(_mapper.Map<AddressDto>);
        }

        public async Task<AddressDto?> GetByIdAsync(Guid id)
        {
            var entity = await _addressRepository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<AddressDto>(entity);
        }

        public async Task<AddressDto> CreateAsync(AddressDto dto)
        {
            var entity = _mapper.Map<Address>(dto);
            var result = await _addressRepository.CreateAsync(entity);
            return _mapper.Map<AddressDto>(result);
        }

        public async Task<AddressDto?> UpdateAsync(AddressDto dto)
        {
            
            var existing = _mapper.Map<Address>(dto);
            var updated = await _addressRepository.UpdateAsync(existing);
            return _mapper.Map<AddressDto>(updated);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _addressRepository.DeleteAsync(id);
        }

        public async Task<AddressDto> SetDefaultAddress(Guid id)
        {
            var update = await _addressRepository.GetByIdAsync(id);
            update.IsDefaultShipping = true;
            var edited = await _addressRepository.SetDefaultAddress(update);
            return _mapper.Map<AddressDto>(edited);
        }
    }
}
