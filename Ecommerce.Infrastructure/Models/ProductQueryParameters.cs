
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Infrastructure.Models
{
    public class ProductQueryParameters : PaginationRequest
    {
        public Guid? CategoryId { get; set; }
        public string? SearchTerm { get; set; }
        public string? Include { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; } = "asc";
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? IsActive { get; set; }

        public ProductQueryParameters(int pageSize = 10, int pageIndex = 1) : base(pageSize, pageIndex) { }
    }
}
