using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Entity
{
    public class Product : BaseEntity // Kế thừa BaseEntity để có Id, CreatedAt, UpdatedAt
    {
        [Required] // Bắt buộc phải có tên
        [MaxLength(100)] // Giới hạn độ dài tên
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; } // Mô tả sản phẩm, có thể null

        [Required] // Bắt buộc phải có giá
        [Column(TypeName = "decimal(18, 2)")] // Kiểu dữ liệu phù hợp cho tiền tệ
        public decimal Price { get; set; }

        // Thuộc tính Quantity gốc đã được comment out hoặc xóa bỏ
        // để chuyển sang quản lý tồn kho chi tiết hơn theo cửa hàng
        // public int Quantity { get; set; }

        public bool IsActive { get; set; } = true; // Trạng thái kích hoạt của sản phẩm

        // --- Foreign Keys ---
        // Khóa ngoại liên kết đến Category
        public Guid CategoryId { get; set; }

        // Khóa ngoại liên kết đến Manufacturer
        public Guid ManufacturerId { get; set; }

        // Khóa ngoại liên kết đến Discount (có thể null nếu sản phẩm không có giảm giá)
        public Guid? DiscountId { get; set; }

        // --- Navigation Properties ---
        // Giúp truy cập dễ dàng đến các đối tượng liên quan thông qua ORM

        [ForeignKey("CategoryId")] // Chỉ định khóa ngoại tương ứng
        public virtual Category? Category { get; set; } // Đối tượng Category liên quan

        [ForeignKey("ManufacturerId")]
        public virtual Manufacturer? Manufacturer { get; set; } // Đối tượng Manufacturer liên quan

        [ForeignKey("DiscountId")]
        public virtual Discount? Discount { get; set; } // Đối tượng Discount liên quan (có thể null)

        // Danh sách các ảnh của sản phẩm (Quan hệ One-to-Many)
        public virtual ICollection<Image> Images { get; set; } = new List<Image>();

        // Danh sách các đánh giá của sản phẩm (Quan hệ One-to-Many)
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        // Danh sách các mục trong đơn hàng chứa sản phẩm này (Quan hệ One-to-Many)
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // Danh sách tồn kho của sản phẩm này tại các cửa hàng (Quan hệ Many-to-Many thông qua ProductStoreInventory)
        public virtual ICollection<ProductStoreInventory> StoreInventories { get; set; } = new List<ProductStoreInventory>();

        // Danh sách các bài viết quảng cáo liên quan đến sản phẩm (Quan hệ Many-to-Many thông qua ProductPromotionPost)
        public virtual ICollection<ProductPost> ProductPosts { get; set; } = new List<ProductPost>();
    }
}
