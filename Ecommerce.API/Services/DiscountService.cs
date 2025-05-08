using AutoMapper;
using Ecommerce.Infrastructure.Models.Dtos;
namespace Ecommerce.API.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository _repository;
        private readonly IMapper _mapper;

        public DiscountService(IDiscountRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DiscountDto>> GetAllAsync()
        {
            var discounts = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<DiscountDto>>(discounts);
        }

        public async Task<DiscountDto?> GetByIdAsync(Guid id)
        {
            var discount = await _repository.GetByIdAsync(id);
            return discount == null ? null : _mapper.Map<DiscountDto>(discount);
        }

        public async Task<DiscountDto> CreateAsync(DiscountDto dto)
        {
            var entity = _mapper.Map<Discount>(dto);
            entity.Id = Guid.NewGuid(); // Tạo ID mới cho Discount

            var result = await _repository.AddAsync(entity);
            return _mapper.Map<DiscountDto>(result);
        }

        public async Task<bool> UpdateAsync(Guid id, DiscountDto dto)
        {
            var discount = await _repository.GetByIdAsync(id);
            if (discount == null) return false;

            // Cập nhật dữ liệu discount từ dto
            _mapper.Map(dto, discount);

            return await _repository.UpdateAsync(discount);
        }

        public async Task<bool> DeleteAsync(Guid id) => await _repository.DeleteAsync(id);
    }
}
