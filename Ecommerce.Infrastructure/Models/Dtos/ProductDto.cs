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
    }

    public class ProductCreateDto
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid CategoryId { get; set; }
        public Guid ManufacturerId { get; set; }
        public Guid? DiscountId { get; set; }
    }

    public class ProductUpdateDto : ProductCreateDto
    {
        public Guid Id { get; set; }

    }

}
