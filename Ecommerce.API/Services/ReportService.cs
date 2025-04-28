using Ecommerce.Infrastructure.Models.Dtos.Reports;
using Ecommerce.API.Services.Interfaces;
using Ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Services
{
    // Service xử lý các báo cáo thống kê
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        // Constructor nhận vào ApplicationDbContext để truy cập cơ sở dữ liệu
        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách sản phẩm bán chạy nhất
        /// </summary>
        /// <param name="top"></param>
        public async Task<List<TopSellingProductDto>> GetTopSellingProductsAsync(int top)
        {
            var query = await _context.OrderItems
                .Include(oi => oi.Product) // Lấy thông tin sản phẩm
                .Where(oi => oi.Order.Status == Ecommerce.Infrastructure.Enums.OrderStatus.COMPLETED) // Chỉ tính các đơn hàng đã hoàn thành
                .GroupBy(oi => new { oi.ProductId, oi.Product.Name }) // Nhóm theo sản phẩm
                .Select(g => new TopSellingProductDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name ?? string.Empty, // Tên sản phẩm, mặc định là chuỗi rỗng nếu không có tên
                    TotalQuantitySold = g.Sum(x => x.Quantity), // Tổng số lượng sản phẩm đã bán
                    TotalRevenue = g.Sum(x => x.TotalItemPrice) // Tổng doanh thu từ sản phẩm này
                })
                .OrderByDescending(x => x.TotalQuantitySold) // Sắp xếp theo số lượng bán được, từ cao đến thấp
                .Take(top) // Lấy số lượng sản phẩm theo tham số `top`
                .ToListAsync();

            return query;
        }

        /// <summary>
        /// Lấy thống kê số lượng đơn hàng theo trạng thái
        /// </summary>
        public async Task<List<OrderStatusStatisticsDto>> GetOrderStatusStatisticsAsync()
        {
            var query = await _context.Orders
                .GroupBy(o => o.Status) // Nhóm theo trạng thái đơn hàng
                .Select(g => new OrderStatusStatisticsDto
                {
                    Status = g.Key.ToString(), // Trạng thái đơn hàng (PENDING, COMPLETED, CANCEL, v.v.)
                    Count = g.Count() // Số lượng đơn hàng theo trạng thái
                })
                .ToListAsync();

            return query;
        }

        /// <summary>
        /// Lấy báo cáo tổng doanh thu
        /// </summary>
        /// <returns></returns>
        public async Task<RevenueReportDto> GetRevenueReportAsync()
        {
            var totalRevenue = await _context.OrderItems
                .Where(oi => oi.Order.Status == Ecommerce.Infrastructure.Enums.OrderStatus.COMPLETED) // Chỉ tính các đơn hàng đã hoàn thành
                .SumAsync(oi => oi.TotalItemPrice); // Tính tổng doanh thu từ các sản phẩm trong đơn hàng

            var totalOrders = await _context.Orders
                .Where(o => o.Status == Ecommerce.Infrastructure.Enums.OrderStatus.COMPLETED) // Chỉ tính các đơn hàng đã hoàn thành
                .CountAsync(); // Tính tổng số đơn hàng đã hoàn thành

            var totalProductsSold = await _context.OrderItems
                .Where(oi => oi.Order.Status == Ecommerce.Infrastructure.Enums.OrderStatus.COMPLETED) // Chỉ tính các sản phẩm trong đơn hàng đã hoàn thành
                .SumAsync(oi => oi.Quantity); // Tính tổng số lượng sản phẩm đã bán

            return new RevenueReportDto
            {
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                TotalProductsSold = totalProductsSold
            };
        }

        /// <summary>
        /// Lấy báo cáo tổng số đơn hàng theo ngày
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public async Task<List<DailyOrderReportDto>> GetDailyOrderReportAsync(DateTime startDate, DateTime endDate)
        {
            var query = await _context.Orders
                .Where(o => o.CreatedDate >= startDate && o.CreatedDate <= endDate) // Lọc đơn hàng theo ngày
                .GroupBy(o => o.CreatedDate.Date) // Nhóm theo ngày tạo đơn hàng
                .Select(g => new DailyOrderReportDto
                {
                    Date = g.Key, // Ngày tạo đơn hàng
                    TotalOrders = g.Count() // Số lượng đơn hàng trong ngày
                })
                .ToListAsync();

            return query;
        }

        /// <summary>
        /// Lấy báo cáo tổng doanh thu theo ngày
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public async Task<List<DailyRevenueReportDto>> GetDailyRevenueReportAsync(DateTime startDate, DateTime endDate)
        {
            var query = await _context.Orders
                .Where(o => o.CreatedDate >= startDate && o.CreatedDate <= endDate) // Lọc đơn hàng theo ngày
                .GroupBy(o => o.CreatedDate.Date) // Nhóm theo ngày tạo đơn hàng
                .Select(g => new DailyRevenueReportDto
                {
                    Date = g.Key, // Ngày tạo đơn hàng
                    TotalRevenue = g.Sum(o => o.TotalAmount) // Tính tổng doanh thu theo ngày
                })
                .ToListAsync();

            return query;
        }

        /// <summary>
        /// Lấy báo cáo tổng quan (số lượng đơn hàng, doanh thu, số người dùng mới, v.v.)
        /// </summary>
        /// <param name="date"></param>
        public async Task<OverviewReportDto> GetOverviewReportAsync(DateTime? date = null)
        {
            var totalOrders = await _context.Orders
                .Where(o => !date.HasValue || o.CreatedDate.Date == date.Value.Date) // Lọc đơn hàng theo ngày nếu có tham số ngày
                .CountAsync(); // Tính tổng số đơn hàng

            var totalRevenue = await _context.OrderItems
                .Where(oi => oi.Order.Status == Ecommerce.Infrastructure.Enums.OrderStatus.COMPLETED) // Chỉ tính các đơn hàng đã hoàn thành
                .SumAsync(oi => oi.TotalItemPrice); // Tính tổng doanh thu

            var totalNewUsers = await _context.Users
                .Where(u => !date.HasValue || u.CreatedDate.Date == date.Value.Date) // Lọc người dùng mới theo ngày nếu có tham số ngày
                .CountAsync(); // Tính số người dùng mới

            return new OverviewReportDto
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                TotalNewUsers = totalNewUsers
            };
        }
    }
}