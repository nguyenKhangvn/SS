using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Entity
{
    public class StoreLocation : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Address { get; set; } // Nullable

        [MaxLength(50)]
        public string? PhoneNumber { get; set; } // Nullable

        // --- Thêm tọa độ ---
        public double? Latitude { get; set; } // Vĩ độ (Nullable)
        public double? Longitude { get; set; } // Kinh độ (Nullable)
        // Bạn cần có quy trình để cập nhật tọa độ này khi thêm/sửa cửa hàng

        // --- Thêm navigation property cho quan hệ Many-to-Many ---
        public virtual ICollection<ProductStoreInventory> ProductInventories { get; set; } = new List<ProductStoreInventory>();
    }
}
