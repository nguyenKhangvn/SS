// Thống kê doanh thu

using System;


namespace Ecommerce.Infrastructure.Models.Dtos.Reports
{
    public class RevenueReportDto
    {
        public decimal TotalRevenue { get; set; }       // Tổng doanh thu
        public int TotalOrders { get; set; }             // Tổng số đơn hàng
        public int TotalProductsSold { get; set; }       // Tổng số lượng sản phẩm đã bán
    }
}