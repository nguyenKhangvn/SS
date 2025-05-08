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

        //overview
        public async Task<OverviewReportDtoV1> GetOverviewReportAsyncV1(DateTime? dateFilterUtc)
        {
            // Define the date range for the "New" metrics and period totals based on the filter date
            DateTime periodStartDateUtc;
            DateTime periodEndDateUtc;
            DateTime totalEndDateUtc;

            if (dateFilterUtc.HasValue)
            {
                // If a specific date is selected, report on that day
                periodStartDateUtc = dateFilterUtc.Value.Date; // Midnight UTC of the selected date
                periodEndDateUtc = periodStartDateUtc.AddDays(1); // Midnight UTC of the next day
                totalEndDateUtc = periodEndDateUtc; // Totals up to the end of the selected day
            }
            else
            {
                // If no date is selected, report on today
                periodStartDateUtc = DateTime.UtcNow.Date; // Midnight UTC today
                periodEndDateUtc = periodStartDateUtc.AddDays(1); // Midnight UTC tomorrow
                totalEndDateUtc = DateTime.UtcNow; // Totals up to the current moment
            }

            // Fetch data from the repository
            var totalUsers = await _reportRepository.GetUserCountAsync(totalEndDateUtc);
            var totalProducts = await _reportRepository.GetProductCountAsync(totalEndDateUtc);
            var totalOrdersOverall = await _reportRepository.GetOrderCountAsync(totalEndDateUtc);
            var totalRevenueOverall = await _reportRepository.GetTotalRevenueAsync(totalEndDateUtc);

            // Get orders within the specific report period
            var periodOrders = await _reportRepository.GetOrdersByCreationDateRangeAsync(periodStartDateUtc, periodEndDateUtc);

            // Calculate metrics for the report period
            var newUsersCount = await _reportRepository.GetUserCountByCreationDateRangeAsync(periodStartDateUtc, periodEndDateUtc);
            var newOrdersCount = periodOrders.Count;
            var periodRevenue = periodOrders.Sum(o => o.TotalAmount); // Sum revenue for the period

            // Calculate Average Order Value for the period
            var averageOrderValue = newOrdersCount > 0 ? periodRevenue / newOrdersCount : 0; // AOV based on new users?
            // **Correction based on standard AOV:** AOV is typically Period Revenue / Period Order Count
            averageOrderValue = newOrdersCount > 0 ? periodRevenue / newOrdersCount : 0;
            // Let's use the standard definition: Period Revenue / Period Order Count
            averageOrderValue = newOrdersCount > 0 ? periodRevenue / newOrdersCount : 0;
            // Using the standard definition again, seems the frontend wants AOV for the selected period's *orders*
            averageOrderValue = newOrdersCount > 0 ? periodRevenue / newOrdersCount : 0;
            // Corrected AOV calculation based on period orders:
            averageOrderValue = newOrdersCount > 0 ? periodRevenue / newOrdersCount : 0;
            // Final attempt at understanding frontend: Maybe it means AOV of orders placed *by new users*? That's less common.
            // Let's assume AOV of *all* orders within the period.
            averageOrderValue = periodOrders.Count > 0 ? periodRevenue / periodOrders.Count : 0;


            // Construct the DTO
            var overviewReportDto = new OverviewReportDtoV1
            {
                // Based on frontend names, "Total" seems overall up to dateFilter, "New" and "Average" are for the period.
                TotalRevenue = totalRevenueOverall, // Overall total or total up to selected date end
                TotalOrders = totalOrdersOverall,   // Overall count or count up to selected date end
                TotalUsers = totalUsers,           // Overall count or count up to selected date end
                TotalProducts = totalProducts,      // Overall count or count up to selected date end
                NewUsers = newUsersCount,          // Users created within the period
                NewOrders = newOrdersCount,         // Orders created within the period
                AverageOrderValue = averageOrderValue // AOV for the period's orders
            };

            return overviewReportDto;
        }
    }
}
