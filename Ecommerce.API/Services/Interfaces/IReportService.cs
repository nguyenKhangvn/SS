using Ecommerce.Infrastructure.Models.Dtos.Reports;

namespace Ecommerce.API.Services.Interfaces
{
    public interface IReportService
    {
        /// <summary>
        /// Lấy danh sách sản phẩm bán chạy nhất.
        /// </summary>
        Task<List<TopSellingProductDto>> GetTopSellingProductsAsync(int top);

        /// <summary>
        /// Lấy tổng số đơn hàng theo ngày.
        /// </summary>
        Task<List<DailyOrderReportDto>> GetDailyOrderReportAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Lấy tổng doanh thu theo ngày.
        /// </summary>
        Task<List<DailyRevenueReportDto>> GetDailyRevenueReportAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Lấy tổng quan thống kê (số lượng đơn, doanh thu, số user mới,...).
        /// </summary>
        Task<OverviewReportDto> GetOverviewReportAsync(DateTime? date = null);

        /// <summary>
        /// Lấy thống kê đơn hàng theo trạng thái.
        /// </summary>
        Task<List<OrderStatusStatisticsDto>> GetOrderStatusStatisticsAsync();

        /// <summary>
        /// Lấy thống kê tổng doanh thu của hệ thống.
        /// </summary>
        Task<RevenueReportDto> GetRevenueReportAsync();
    }
}