using AutoMapper;
using Ecommerce.API.Repositories;
using Ecommerce.API.Services.Interfaces;
using Ecommerce.Infrastructure.Entity;
using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services
{
    public class ManufacturerService : IManufacturerService
    {
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly IMapper _mapper;

        public ManufacturerService(IManufacturerRepository manufacturerRepository, IMapper mapper)
        {
            _manufacturerRepository = manufacturerRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ManufacturerDto>> GetAllAsync()
        {
            var manufacturers = await _manufacturerRepository.GetAllAsync();
            return manufacturers.Select(m => _mapper.Map<ManufacturerDto>(m));
        }

        public async Task<ManufacturerDto?> GetByIdAsync(Guid id)
        {
            var manufacturer = await _manufacturerRepository.GetByIdAsync(id);
            if (manufacturer == null) return null;

            return _mapper.Map<ManufacturerDto>(manufacturer);
        }

        public async Task<ManufacturerDto> CreateAsync(ManufacturerDto dto)
        {
            var entity = _mapper.Map<Manufacturer>(dto);
            var created = await _manufacturerRepository.CreateAsync(entity);
            return _mapper.Map<ManufacturerDto>(created);
        }

        public async Task<ManufacturerDto?> UpdateAsync(Guid id, ManufacturerDto dto)
        {
            var manufatureEntity = _mapper.Map<Manufacturer>(dto);
            var updated = await _manufacturerRepository.UpdateAsync(manufatureEntity);

            return _mapper.Map<ManufacturerDto>(updated);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _manufacturerRepository.DeleteAsync(id);
        }
    }
}
