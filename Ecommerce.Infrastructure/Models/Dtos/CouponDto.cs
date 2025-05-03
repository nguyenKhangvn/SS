using Ecommerce.Infrastructure.Entity;
using System;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Infrastructure.Models.Dtos
{
    public class CouponDto
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required]
        public DiscountType DiscountType { get; set; }
        [Required]
        public decimal Value { get; set; }
        public decimal MinimumSpend { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? UsageLimit { get; set; }
        public int? UsageLimitPerUser { get; set; }
        public bool IsActive { get; set; }
        public Guid? UserId { get; set; }
    }

    public class CouponCreateDto
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required]
        public DiscountType DiscountType { get; set; }
        [Required]
        public decimal Value { get; set; }
        public decimal MinimumSpend { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? UsageLimit { get; set; }
        public int? UsageLimitPerUser { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid? UserId { get; set; }
    }

    public class ApplyCouponResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
    }

    public class SaveCouponResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class ApplyCouponRequest
    {
        public string Code { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
        public decimal OrderTotal { get; set; }
    }

    public class SaveCouponRequest
    {
        public Guid UserId { get; set; }
        public string CouponCode { get; set; } = string.Empty;
    }
}