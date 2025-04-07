using AutoMapper;
using Ecommerce.Infrastructure.Models.Dtos;
using System.Net.WebSockets;

namespace Ecommerce.API.Services
{
    public class StoreLocationService: IStoreLocationService
    {
        private readonly IStoreLocationRepository _storeLocationRepository;
        private readonly IMapper _mapper;
        public StoreLocationService(IStoreLocationRepository storeLocationRepository, IMapper mapper)
        {
            _storeLocationRepository = storeLocationRepository;
            _mapper = mapper;
        }
        public async Task<StoreLocationDto> AddAsync(StoreLocationDto storeLocation)
        {
            var storeLocationEntity = _mapper.Map<StoreLocation>(storeLocation);
            var addedStoreLocation = await _storeLocationRepository.AddAsync(storeLocationEntity);
            return _mapper.Map<StoreLocationDto>(addedStoreLocation);
        }
        public async Task<StoreLocationDto> UpdateAsync(StoreLocationDto storeLocation)
        {
            var storeLocationEntity = _mapper.Map<StoreLocation>(storeLocation);
            var updatedStoreLocation = await _storeLocationRepository.UpdateAsync(storeLocationEntity);
            return _mapper.Map<StoreLocationDto>(updatedStoreLocation);
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            var storeLocation = await _storeLocationRepository.GetByIdAsync(id);
            if (storeLocation == null)
            {
                return false;
            }
            return await _storeLocationRepository.DeleteAsync(id);
        }

        public async Task<StoreLocationDto?> GetByIdAsync(Guid id, string includeProperties)
        {
            var storeLocation = await _storeLocationRepository.GetByIdAsync(id, includeProperties);
            if (storeLocation == null)
            {
                return null;
            }
            return _mapper.Map<StoreLocationDto>(storeLocation);
        }

        public async Task<IEnumerable<StoreLocationDto>> GetStoreLocations(string? includeProperties = null)
        {
            var query = await _storeLocationRepository.GetStoreLocationsAsync(includeProperties);
            return query.Select(sl => _mapper.Map<StoreLocationDto>(sl));
        }
    }
  
}
