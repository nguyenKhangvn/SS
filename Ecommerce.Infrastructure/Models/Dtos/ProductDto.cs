using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Infrastructure.Models.Dtos
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public string CategoryName { get; set; } = default!;
        public string ManufacturerName { get; set; } = default!;
        public string? DiscountName { get; set; }
        public string Slug { get; set; } = default!;
        public List<ImageDto> Images { get; set; } = new List<ImageDto>();
        public int Quantity { get; set; }
        public string StoreName { get; set; } = default!;
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }

    // DTO cho thông tin tồn kho tại từng cửa hàng
    public class StoreInventoryDto
    {
        public Guid StoreId { get; set; }
        public string StoreName { get; set; } = default!;
        public int Quantity { get; set; }
        public string? StoreAddress { get; set; }
    }

    public class ProductCreateDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = default!;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;

        [StringLength(200)]
        public string? Slug { get; set; } = default!;

        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        public Guid ManufacturerId { get; set; }

        public Guid? DiscountId { get; set; }

        // Thêm thông tin tồn kho ban đầu
        
        public IFormFile? ImageFile1 { get; set; }
        public IFormFile? ImageFile2 { get; set; }
        public IFormFile? ImageFile3 { get; set; }
        public IFormFile? ImageFile4 { get; set; }
        public Guid StoreId { get; set; }
        public int Quantity { get; set; } // Tổng số lượng tồn kho trên tất cả cửa hàng
    }

    // DTO cho số lượng tồn kho ban đầu khi tạo sản phẩm
    public class InitialInventoryDto
    {
        [Required]
        public Guid StoreId { get; set; }

        [Range(0, int.MaxValue)]
        public int InitialQuantity { get; set; }
    }

    public class ProductUpdateDto : ProductCreateDto
    {
        // Có thể thêm các trường đặc biệt cho update nếu cần
    }
}