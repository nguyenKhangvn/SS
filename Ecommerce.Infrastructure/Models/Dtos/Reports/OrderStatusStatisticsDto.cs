// Thống kê đơn hàng theo trạng thái

using System;

namespace Ecommerce.Infrastructure.Models.Dtos.Reports
{
    public class OrderStatusStatisticsDto
    {
        public string Status { get; set; } = string.Empty;   // Tên trạng thái (PENDING, SUCCESS, CANCEL,...)
        public int Count { get; set; }                       // Số lượng đơn ở trạng thái này
    }
}
