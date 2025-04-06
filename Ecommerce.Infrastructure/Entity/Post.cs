using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Entity
{
    public class Post : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        public string? Content { get; set; } // Nullable, potentially TEXT type in DB

        [Required]
        [MaxLength(255)]
        public string Uri { get; set; } = string.Empty; // Slug for URL

        public DateTime? StartTime { get; set; } // Nullable
        public DateTime? EndTime { get; set; } // Nullable
        public bool IsPublished { get; set; } = false;

        // Navigation Properties
        public virtual ICollection<ProductPost> ProductPosts { get; set; } = new List<ProductPost>(); // Many-to-Many join entity
    }
}
