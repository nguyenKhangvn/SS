using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Entity
{
    public class OrderItem : BaseEntity
    {
        // Foreign Keys
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PriceAtOrder { get; set; } // Price when ordered

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalItemPrice { get; set; } // quantity * priceAtOrder

        // Navigation Properties
        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}
