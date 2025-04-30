using Ecommerce.Infrastructure.Models.Dtos.Reports;
using Ecommerce.API.Services.Interfaces;

namespace Ecommerce.API.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;

        /// <summary>
        /// Khởi tạo ReportService với dependency IReportRepository
        /// </summary>
        /// <param name="reportRepository"></param>
        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        /// <summary>
        /// Lấy danh sách sản phẩm bán chạy nhất
        /// </summary>
        /// <param name="top"></param>
        public async Task<List<TopSellingProductDto>> GetTopSellingProductsAsync(int top)
        {
            return await _reportRepository.GetTopSellingProductsAsync(top);
        }

        /// <summary>
        /// Lấy thống kê đơn hàng theo trạng thái
        /// </summary>
        public async Task<List<OrderStatusStatisticsDto>> GetOrderStatusStatisticsAsync()
        {
            return await _reportRepository.GetOrderStatusStatisticsAsync();
        }

        /// <summary>
        /// Lấy thống kê tổng doanh thu của hệ thống
        /// </summary>
        /// <returns></returns>
        public async Task<RevenueReportDto> GetRevenueReportAsync()
        {
            return await _reportRepository.GetRevenueReportAsync();
        }

        /// <summary>
        /// Lấy tổng số đơn hàng theo ngày trong khoảng thời gian
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public async Task<List<DailyOrderReportDto>> GetDailyOrderReportAsync(DateTime startDate, DateTime endDate)
        {
            return await _reportRepository.GetDailyOrderReportAsync(startDate, endDate);
        }

        /// <summary>
        /// Lấy tổng doanh thu theo ngày trong khoảng thời gian
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public async Task<List<DailyRevenueReportDto>> GetDailyRevenueReportAsync(DateTime startDate, DateTime endDate)
        {
            return await _reportRepository.GetDailyRevenueReportAsync(startDate, endDate);
        }

        /// <summary>
        /// Lấy tổng quan thống kê (số lượng đơn, doanh thu, số user mới,...) theo ngày (mặc định là ngày hiện tại nếu không truyền vào)
        /// </summary>
        /// <param name="date"></param>
        public async Task<OverviewReportDto> GetOverviewReportAsync(DateTime? date = null)
        {
            return await _reportRepository.GetOverviewReportAsync(date);
        }
    }
}
