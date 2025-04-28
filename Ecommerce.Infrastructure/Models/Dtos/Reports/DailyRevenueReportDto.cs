//Tổng doanh thu mỗi ngày.

using System;

namespace Ecommerce.Infrastructure.Models.Dtos.Reports
{
    public class DailyRevenueReportDto
    {
        public DateTime Date { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}