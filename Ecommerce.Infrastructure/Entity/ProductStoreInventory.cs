using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Entity
{
    public class ProductStoreInventory : BaseEntity // Kế thừa BaseEntity nếu cần theo dõi lịch sử tồn kho
    {
        // Foreign Keys
        public Guid ProductId { get; set; }
        public Guid StoreLocationId { get; set; }

        [Required]
        public int Quantity { get; set; } // Số lượng tồn kho của sản phẩm tại cửa hàng này

        // Navigation Properties
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        [ForeignKey("StoreLocationId")]
        public virtual StoreLocation? StoreLocation { get; set; }
    }
}
