//Tổng số đơn hàng mỗi ngày.

namespace Ecommerce.Infrastructure.Models.Dtos.Reports
{
    public class DailyOrderReportDto
    {
        public DateTime Date { get; set; }
        public int TotalOrders { get; set; }
    }
}