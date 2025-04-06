using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Entity
{
    public class Shipment : BaseEntity
    {
        // Foreign Key
        public Guid OrderId { get; set; }

        [MaxLength(100)]
        public string? TrackingNumber { get; set; } // Nullable

        [MaxLength(100)]
        public string? Carrier { get; set; } // Nullable, e.g., 'VNPost', 'GHTK'

        [Required]
        public ShipmentStatus Status { get; set; } = ShipmentStatus.PENDING;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ShippingCost { get; set; } = 0;

        public DateTime? ShippedAt { get; set; } // Nullable
        public DateTime? EstimatedDelivery { get; set; } // Nullable
        public DateTime? DeliveredAt { get; set; } // Nullable

        // Navigation Property
        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }
    }
}
