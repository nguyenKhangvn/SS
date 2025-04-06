using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Entity
{
    public class ProductPost : BaseEntity // Kế thừa BaseEntity để có Id, CreatedAt, UpdatedAt
    {
        public Guid ProductId { get; set; }
        public Guid PostId { get; set; }

        // Navigation properties back to the entities
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
        [ForeignKey("PostId")]
        public virtual Post? Post { get; set; }
    }
}
