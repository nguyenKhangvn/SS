// Yhống kê các sản phẩm bán chạy nhất

namespace Ecommerce.Infrastructure.Models.Dtos.Reports
{
        public class TopSellingProductDto
        {
            public Guid ProductId { get; set; }              // ID sản phẩm
            public string ProductName { get; set; } = string.Empty;  // Tên sản phẩm
            public int TotalQuantitySold { get; set; }       // Tổng số lượng đã bán
            public decimal TotalRevenue { get; set; }        // Tổng doanh thu từ sản phẩm đó
        }
}
