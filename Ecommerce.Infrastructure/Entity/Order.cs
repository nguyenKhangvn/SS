using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Entity
{
    public class Order : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string OrderCode { get; set; } = string.Empty; // User-friendly code

        // Foreign Keys
        public Guid UserId { get; set; }
        public Guid ShippingAddressId { get; set; }
        public Guid? BillingAddressId { get; set; } // Nullable if same as shipping
        public Guid? CouponId { get; set; } // Nullable
        public Guid? PaymentId { get; set; } // Link to payment record

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.PENDING;

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ShippingCost { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal DiscountAmount { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        public string? Notes { get; set; } // Nullable

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("ShippingAddressId")]
        public virtual Address? ShippingAddress { get; set; }

        [ForeignKey("BillingAddressId")]
        public virtual Address? BillingAddress { get; set; }

        [ForeignKey("CouponId")]
        public virtual Coupon? Coupon { get; set; }

        [ForeignKey("PaymentId")]
        public virtual Payment? Payment { get; set; } // Could be One-to-One configured in DbContext

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    }
}
