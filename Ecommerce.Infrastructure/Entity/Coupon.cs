using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Entity
{
    public class Coupon : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        public string? Description { get; set; } // Nullable

        [Required]
        public DiscountType DiscountType { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Value { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal MinimumSpend { get; set; } = 0;

        public DateTime? StartTime { get; set; } // Nullable
        public DateTime? EndTime { get; set; } // Nullable

        public int? UsageLimit { get; set; } // Nullable
        public int? UsageLimitPerUser { get; set; } // Nullable
        public bool IsActive { get; set; } = true;

        // Foreign Key (Nullable if coupon is not user-specific)
        public Guid? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        // Navigation Properties
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>(); // Orders using this coupon
    }
}
