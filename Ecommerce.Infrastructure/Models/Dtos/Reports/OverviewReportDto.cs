//Thống kê tổng quan toàn hệ thống.

using System;

namespace Ecommerce.Infrastructure.Models.Dtos.Reports
{
    public class OverviewReportDto
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalCustomers { get; set; }
        public int NewCustomersToday { get; set; }
        public int TotalProducts { get; set; }
    }

    public class OverviewReportDtoV1
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int NewUsers { get; set; }
        public int NewOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
    }
}