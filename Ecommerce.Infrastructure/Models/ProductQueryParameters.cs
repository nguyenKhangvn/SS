
namespace Ecommerce.Infrastructure.Models
{
    public class ProductQueryParameters : PaginationRequest 
    {
        public Guid? CategoryId { get; set; }

        public string? SearchTerm { get; set; } // Tìm kiếm theo tên hoặc mô tả
        public string? SortBy { get; set; } // Tên trường để sắp xếp (vd: "name", "price")
        public string? SortOrder { get; set; } = "asc"; // Thứ tự sắp xếp ("asc" hoặc "desc")
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? IsActive { get; set; } // Lọc theo trạng thái active

        public ProductQueryParameters(int pageSize = 10, int pageIndex = 0) : base(pageSize, pageIndex) { }
    }
}
