using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Entity
{
    public class Review : BaseEntity
    {
        // Foreign Keys
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }

        [Range(1, 5)] // Ensure stars are between 1 and 5
        public int Stars { get; set; }

        public string? Comment { get; set; } // Nullable

        // Navigation Properties
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
