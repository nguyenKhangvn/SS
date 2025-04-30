using Ecommerce.Infrastructure.Models.Dtos.Reports;

namespace Ecommerce.API.Services.Interfaces
{
    /// <summary>
    /// Interface định nghĩa các phương thức để lấy dữ liệu báo cáo
    /// </summary>
    public interface IReportRepository
    {
        /// <summary>
        /// Lấy danh sách sản phẩm bán chạy nhất (top sản phẩm)
        /// </summary>
        /// <param name="top"></param>
        Task<List<TopSellingProductDto>> GetTopSellingProductsAsync(int top);

        /// <summary>
        /// Lấy thống kê số lượng đơn hàng theo từng trạng thái (PENDING, SUCCESS, CANCEL,...)
        /// </summary>
        Task<List<OrderStatusStatisticsDto>> GetOrderStatusStatisticsAsync();

        /// <summary>
        /// Lấy báo cáo tổng doanh thu, tổng số đơn hàng và tổng số sản phẩm đã bán
        /// </summary>
        Task<RevenueReportDto> GetRevenueReportAsync();

        /// <summary>
        /// Lấy báo cáo số lượng đơn hàng theo từng ngày trong khoảng thời gian
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        Task<List<DailyOrderReportDto>> GetDailyOrderReportAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Lấy báo cáo doanh thu theo từng ngày trong khoảng thời gian
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        Task<List<DailyRevenueReportDto>> GetDailyRevenueReportAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Lấy báo cáo tổng quan (tổng số đơn hàng, doanh thu, khách hàng mới,...) theo ngày (mặc định là ngày hiện tại nếu không truyền vào)
        /// </summary>
        /// <param name="date"></param>
        Task<OverviewReportDto> GetOverviewReportAsync(DateTime? date = null);
    }
}
