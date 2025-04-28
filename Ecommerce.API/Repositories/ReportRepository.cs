using Ecommerce.Infrastructure.Models.Dtos.Reports;
using Ecommerce.Infrastructure.Data;
using Ecommerce.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly EcommerceDbContext _context;

        /// <summary>  
        /// Khởi tạo ReportRepository với EcommerceDbContext  
        /// </summary>  
        /// <param name="context">Đối tượng EcommerceDbContext để truy cập cơ sở dữ liệu</param>  
        public ReportRepository(EcommerceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách sản phẩm bán chạy nhất (top sản phẩm)
        /// </summary>
        /// <param name="top">Số lượng sản phẩm cần lấy</param>
        /// <returns>Danh sách sản phẩm bán chạy nhất</returns>
        public async Task<List<TopSellingProductDto>> GetTopSellingProductsAsync(int top)
        {
            var query = await _context.OrderItems
                .Include(oi => oi.Product)
                .Where(oi => oi.Order.Status == Ecommerce.Infrastructure.Entity.OrderStatus.COMPLETED)
                .GroupBy(oi => new { oi.ProductId, oi.Product.Name })
                .Select(g => new TopSellingProductDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name ?? string.Empty,
                    TotalQuantitySold = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.TotalItemPrice)
                })
                .OrderByDescending(x => x.TotalQuantitySold)
                .Take(top)
                .ToListAsync();

            return query;
        }

        /// <summary>
        /// Lấy thống kê số lượng đơn hàng theo từng trạng thái
        /// </summary>
        /// <returns>Danh sách thống kê trạng thái đơn hàng</returns>
        public async Task<List<OrderStatusStatisticsDto>> GetOrderStatusStatisticsAsync()
        {
            var query = await _context.Orders
                .GroupBy(o => o.Status)
                .Select(g => new OrderStatusStatisticsDto
                {
                    Status = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToListAsync();

            return query;
        }

        /// <summary>
        /// Lấy báo cáo tổng doanh thu, tổng số đơn hàng và tổng số sản phẩm đã bán
        /// </summary>
        /// <returns>Đối tượng chứa thông tin báo cáo doanh thu</returns>
        public async Task<RevenueReportDto> GetRevenueReportAsync()
        {
            var totalRevenue = await _context.OrderItems
                .Where(oi => oi.Order.Status == Ecommerce.Infrastructure.Entity.OrderStatus.COMPLETED)
                .SumAsync(oi => oi.TotalItemPrice);

            var totalOrders = await _context.Orders
                .Where(o => o.Status == Ecommerce.Infrastructure.Entity.OrderStatus.COMPLETED)
                .CountAsync();

            var totalProductsSold = await _context.OrderItems
                .Where(oi => oi.Order.Status == Ecommerce.Infrastructure.Entity.OrderStatus.COMPLETED)
                .SumAsync(oi => oi.Quantity);

            return new RevenueReportDto
            {
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                TotalProductsSold = totalProductsSold
            };
        }

        /// <summary>
        /// Lấy báo cáo số lượng đơn hàng theo từng ngày trong khoảng thời gian
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu</param>
        /// <param name="endDate">Ngày kết thúc</param>
        /// <returns>Danh sách báo cáo số lượng đơn hàng theo ngày</returns>
        public async Task<List<DailyOrderReportDto>> GetDailyOrderReportAsync(DateTime startDate, DateTime endDate)
        {
            var query = await _context.Orders
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new DailyOrderReportDto
                {
                    Date = g.Key,
                    TotalOrders = g.Count()
                })
                .ToListAsync();

            return query;
        }

        /// <summary>
        /// Lấy báo cáo doanh thu theo từng ngày trong khoảng thời gian
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu</param>
        /// <param name="endDate">Ngày kết thúc</param>
        /// <returns>Danh sách báo cáo doanh thu theo ngày</returns>
        public async Task<List<DailyRevenueReportDto>> GetDailyRevenueReportAsync(DateTime startDate, DateTime endDate)
        {
            var query = await _context.Orders
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new DailyRevenueReportDto
                {
                    Date = g.Key,
                    TotalRevenue = g.Sum(o => o.TotalAmount)
                })
                .ToListAsync();

            return query;
        }

        /// <summary>
        /// Lấy báo cáo tổng quan (tổng số đơn hàng, doanh thu) theo ngày
        /// </summary>
        /// <param name="date">Ngày cần lấy báo cáo (mặc định là ngày hiện tại nếu không truyền vào)</param>
        /// <returns>Đối tượng chứa thông tin báo cáo tổng quan</returns>
        public async Task<OverviewReportDto> GetOverviewReportAsync(DateTime? date = null)
        {
            var totalOrders = await _context.Orders
                .Where(o => !date.HasValue || o.CreatedAt.Date == date.Value.Date)
                .CountAsync();

            var totalRevenue = await _context.OrderItems
                .Where(oi => oi.Order.Status == Ecommerce.Infrastructure.Entity.OrderStatus.COMPLETED)
                .SumAsync(oi => oi.TotalItemPrice);

            return new OverviewReportDto
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
            };
        }
    }
}
