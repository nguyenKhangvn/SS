namespace Ecommerce.API.Dtos
{
    public class ProductQueryParameters : PaginationRequest
    {
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; } = "asc";
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? IsActive { get; set; }
        public Guid? CategoryId { get; set; } // Specific filter parameter
        public Guid? ManufacturerId { get; set; } // Specific filter parameter

        public ProductQueryParameters(int pageSize = 10, int pageIndex = 0)
            : base(pageSize, pageIndex) { }
    }
}
