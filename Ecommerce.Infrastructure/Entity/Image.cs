using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Entity
{
    public class Image : BaseEntity
    {
        // Foreign Key
        public Guid ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        [Required]
        [MaxLength(500)] // URL can be long
        public string Url { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? AltText { get; set; } // Nullable
        //chua hieu
        public int DisplayOrder { get; set; } = 0;
    }
}
