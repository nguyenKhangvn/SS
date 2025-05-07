using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IMapper _mapper;
        private readonly EcommerceDbContext _context;

        public CouponService(ICouponRepository couponRepository, IMapper mapper, EcommerceDbContext context)
        {
            _couponRepository = couponRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<CouponDto>> GetAllAsync(Guid? userId, bool onlyActive)
        {
            var coupons = await _couponRepository.GetAllAsync(onlyActive, userId);
            return _mapper.Map<IEnumerable<CouponDto>>(coupons);
        }

        public async Task<IEnumerable<CouponDto>> GetCouponsActiveAsync(bool onlyActive = true, Guid? userId = null)
        {
            var coupons = await _couponRepository.GetCouponsActiveAsync(onlyActive, userId);
            return _mapper.Map<IEnumerable<CouponDto>>(coupons); // Corrected return type to match the method signature
        }


        public async Task<CouponDto?> GetByIdAsync(Guid id)
        {
            var coupon = await _couponRepository.GetByIdAsync(id);
            return coupon != null ? _mapper.Map<CouponDto>(coupon) : null;
        }

        public async Task<CouponDto> AddAsync(CouponCreateDto dto)
        {
            var existingCoupon = await _couponRepository.GetByCodeAsync(dto.Code);
            if (existingCoupon != null)
            {
                throw new InvalidOperationException("Mã coupon đã tồn tại.");
            }

            var coupon = _mapper.Map<Coupon>(dto);
            coupon.Id = Guid.NewGuid();
            var created = await _couponRepository.AddAsync(coupon);
            return _mapper.Map<CouponDto>(created);
        }

        public async Task<CouponDto?> UpdateAsync(Guid id, CouponCreateDto dto)
        {
            var coupon = await _couponRepository.GetByIdAsync(id);
            if (coupon == null)
            {
                return null;
            }

            _mapper.Map(dto, coupon);
            var updatedCoupon = await _couponRepository.UpdateAsync(coupon);
            return _mapper.Map<CouponDto>(updatedCoupon);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _couponRepository.DeleteAsync(id);
        }

        public async Task<ApplyCouponResult> ApplyCouponAsync(string code, Guid? userId, decimal orderTotal)
        {
            var coupon = await _couponRepository.GetByCodeAsync(code);
            if (coupon == null || !coupon.IsActive)
            {
                return new ApplyCouponResult { Success = false, Message = "Mã coupon không tồn tại hoặc không hoạt động.", DiscountAmount = 0 };
            }

            var now = DateTime.UtcNow;

            // Kiểm tra thời gian hiệu lực
            if (coupon.StartTime > now || (coupon.EndTime.HasValue && coupon.EndTime < now))
            {
                return new ApplyCouponResult { Success = false, Message = "Mã coupon đã hết hạn.", DiscountAmount = 0 };
            }

            // Kiểm tra số tiền tối thiểu
            if (orderTotal < coupon.MinimumSpend)
            {
                return new ApplyCouponResult { Success = false, Message = $"Đơn hàng phải đạt tối thiểu {coupon.MinimumSpend:C} để sử dụng mã này.", DiscountAmount = 0 };
            }

            // Kiểm tra UsageLimit (toàn hệ thống)
            if (coupon.UsageLimit.HasValue && coupon.Orders.Count >= coupon.UsageLimit.Value)
            {
                return new ApplyCouponResult { Success = false, Message = "Mã coupon đã được sử dụng hết.", DiscountAmount = 0 };
            }

            // Kiểm tra UsageLimitPerUser
            if (coupon.UsageLimitPerUser.HasValue && userId.HasValue)
            {
                var userUsage = coupon.Orders.Count(o => o.UserId == userId);
                if (userUsage >= coupon.UsageLimitPerUser.Value)
                {
                    return new ApplyCouponResult { Success = false, Message = "Bạn đã sử dụng hết lượt với mã này.", DiscountAmount = 0 };
                }
            }

            // Kiểm tra UserId (coupon cá nhân hóa)
            if (coupon.UserId.HasValue && coupon.UserId != userId)
            {
                return new ApplyCouponResult { Success = false, Message = "Mã coupon không áp dụng cho bạn.", DiscountAmount = 0 };
            }

            // Tính giá trị giảm
            decimal discountAmount = coupon.DiscountType == DiscountType.PERCENTAGE
                ? orderTotal * (coupon.Value / 100)
                : coupon.Value;

            return new ApplyCouponResult { Success = true, Message = "Áp dụng mã coupon thành công.", DiscountAmount = discountAmount };
        }

        public async Task<SaveCouponResult> SaveCouponAsync(Guid userId, string couponCode)
        {
            var coupon = await _couponRepository.GetByCodeAsync(couponCode);
            if (coupon == null || !coupon.IsActive || (coupon.EndTime.HasValue && coupon.EndTime < DateTime.UtcNow))
            {
                return new SaveCouponResult { Success = false, Message = "Mã coupon không tồn tại hoặc đã hết hạn." };
            }

            // Kiểm tra xem người dùng đã lưu coupon này chưa
            var existing = await _context.UserCoupons
                .AnyAsync(uc => uc.UserId == userId && uc.CouponId == coupon.Id);

            if (existing)
            {
                return new SaveCouponResult { Success = false, Message = "Bạn đã lưu mã này rồi." };
            }

            var userCoupon = new UserCoupon
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CouponId = coupon.Id,
                SavedAt = DateTime.UtcNow
            };

            await _couponRepository.SaveCouponAsync(userCoupon);
            return new SaveCouponResult { Success = true, Message = "Lưu mã coupon thành công." };
        }

        public async Task<IEnumerable<CouponDto>> GetSavedCouponsAsync(Guid userId)
        {
            var coupons = await _couponRepository.GetSavedCouponsAsync(userId);
            return _mapper.Map<IEnumerable<CouponDto>>(coupons);
        }

        public async Task<bool> UseAndDeleteAsync(Guid couponId)
        {
            try
            {
                var userCoupon = await _context.UserCoupons
                    .FirstOrDefaultAsync(uc => uc.CouponId == couponId);

                if (userCoupon == null)
                {
                    return false; // Không tìm thấy bản ghi để xóa
                }

                _context.UserCoupons.Remove(userCoupon);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                // Xử lý lỗi (log lỗi nếu cần)
                return false;
            }
        }


    }
}