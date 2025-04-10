using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IMapper _mapper;

        public CouponService(ICouponRepository couponRepository, IMapper mapper)
        {
            _couponRepository = couponRepository;
            _mapper = mapper;
        }
        public async Task<CouponDto> AddAsync(CouponDto coupon)
        {
            var entity =  _mapper.Map<Coupon>(coupon);
            var addedEntity = await _couponRepository.AddAsync(entity);
            return _mapper.Map<CouponDto>(addedEntity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _couponRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<CouponDto>> GetAllAsync()
        {
            var entities = await _couponRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CouponDto>>(entities);
        }

        public async Task<CouponDto?> GetByIdAsync(Guid id)
        {
            var entity = await _couponRepository.GetByIdAsync(id);
            return _mapper.Map<CouponDto>(entity);
        }

        public async Task<CouponDto> UpdateAsync(CouponDto coupon)
        {
            var entity = _mapper.Map<Coupon>(coupon);
            var updatedEntity = await _couponRepository.UpdateAsync(entity);
            return _mapper.Map<CouponDto>(updatedEntity);
        }
    }
}
