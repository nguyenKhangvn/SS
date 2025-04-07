using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Models.Dtos
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public string? ParentCategoryName { get; set; } 
        public int ProductCount { get; set; }
        public int SubCategoryCount { get; set; }
    }

    public class CategoryDetailDto : CategoryDto
    {
        public List<CategoryDto> SubCategories { get; set; } = new List<CategoryDto>();
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }

    public class CreateCategoryDto
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Tên danh mục là bắt buộc.")]
        [MaxLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự.")]
        public string? Description { get; set; }

        public Guid? ParentCategoryId { get; set; }
    }

    public class CategoryTreeNodeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<CategoryTreeNodeDto> Children { get; set; } = new List<CategoryTreeNodeDto>();
    }
}
