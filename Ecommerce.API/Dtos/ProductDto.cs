using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.Dtos
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; } // Include related name
        public Guid ManufacturerId { get; set; }
        public string? ManufacturerName { get; set; } // Include related name
        public Guid? DiscountId { get; set; }
        public string? DiscountCode { get; set; } // Include related info
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        // Add other fields as needed, e.g., average rating, image URLs
        // public double AverageRating { get; set; }
        // public List<string> ImageUrls { get; set; } = new List<string>();

    }

    public class CreateProductDto
    {
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        [MaxLength(100, ErrorMessage = "Tên sản phẩm không được vượt quá 100 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc.")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0.")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Danh mục là bắt buộc.")]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "Nhà sản xuất là bắt buộc.")]
        public Guid ManufacturerId { get; set; }

        public Guid? DiscountId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // DTO for updating an existing product
    public class UpdateProductDto
    {
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        [MaxLength(100, ErrorMessage = "Tên sản phẩm không được vượt quá 100 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc.")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0.")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Danh mục là bắt buộc.")]
        public Guid CategoryId { get; set; }

        [Required(ErrorMessage = "Nhà sản xuất là bắt buộc.")]
        public Guid ManufacturerId { get; set; }

        public Guid? DiscountId { get; set; }
        public bool IsActive { get; set; }
    }
}
