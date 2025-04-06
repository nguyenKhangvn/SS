using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Entity
{
    public class Payment : BaseEntity
    {
        // Foreign Key (May also be configured as One-to-One with Order)
        public Guid OrderId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; } = string.Empty; // e.g., 'CREDIT_CARD', 'COD'

        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.PENDING;

        [MaxLength(100)]
        public string? TransactionId { get; set; } // From payment gateway, Nullable

        public DateTime? PaidAt { get; set; } // Nullable

        // Navigation Property
        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }
    }
}
