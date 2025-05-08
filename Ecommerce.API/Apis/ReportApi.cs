using Ecommerce.Infrastructure.Models.Dtos.Reports;
using Ecommerce.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Apis
{
    // Lớp ReportApi dùng để đăng ký các API endpoint liên quan đến báo cáo
    public static class ReportApi
    {
        // Phương thức MapReportApi đăng ký các endpoint API báo cáo cho ứng dụng
        public static IEndpointRouteBuilder MapReportApi(this IEndpointRouteBuilder builder)
        {
            var vApi = builder.NewVersionedApi("ecommerce"); // Đăng ký phiên bản API
            var v1 = vApi.MapGroup("api/v{version:apiVersion}/ecommerce/reports").HasApiVersion(1, 0); // Đặt phiên bản API là v1

            // [GET] Endpoint lấy danh sách sản phẩm bán chạy nhất
            // http://localhost:5000/api/v1/ecommerce/reports/top-selling-products
            v1.MapGet("/top-selling-products", async (IReportService reportService, [FromQuery] int top) =>
            {
                var result = await reportService.GetTopSellingProductsAsync(top);
                // Trả về kết quả nếu có dữ liệu, nếu không trả về mã NoContent (không có nội dung)
                return result.Any() ? Results.Ok(result) : Results.NoContent();
            });

            // [GET] Endpoint lấy thống kê trạng thái đơn hàng
            // http://localhost:5000/api/v1/ecommerce/reports/order-status-statistics
            v1.MapGet("/order-status-statistics", async (IReportService reportService) =>
            {
                var result = await reportService.GetOrderStatusStatisticsAsync();
                // Trả về kết quả nếu có dữ liệu, nếu không trả về mã NoContent (không có nội dung)
                return result.Any() ? Results.Ok(result) : Results.NoContent();
            });

            // [GET] Endpoint lấy báo cáo doanh thu
            // http://localhost:5000/api/v1/ecommerce/reports/revenue-report
            v1.MapGet("/revenue-report", async (IReportService reportService) =>
            {
                var result = await reportService.GetRevenueReportAsync();
                // Trả về kết quả nếu có dữ liệu, nếu không trả về mã NotFound (không tìm thấy)
                return result != null ? Results.Ok(result) : Results.NotFound();
            });

            // [GET] Endpoint lấy báo cáo số đơn hàng theo ngày
            // http://localhost:5000/api/v1/reports/daily-order-report
            v1.MapGet("/daily-order-report", async (IReportService reportService, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate) =>
            {
                var result = await reportService.GetDailyOrderReportAsync(startDate, endDate);
                // Trả về kết quả nếu có dữ liệu, nếu không trả về mã NoContent (không có nội dung)
                return result.Any() ? Results.Ok(result) : Results.NoContent();
            });

            // [GET] Endpoint lấy báo cáo doanh thu theo ngày
            // http://localhost:5000/api/v1/ecommerce/reports/daily-revenue-report
            v1.MapGet("/daily-revenue-report", async (IReportService reportService, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate) =>
            {
                var result = await reportService.GetDailyRevenueReportAsync(startDate, endDate);
                // Trả về kết quả nếu có dữ liệu, nếu không trả về mã NoContent (không có nội dung)
                return result.Any() ? Results.Ok(result) : Results.NoContent();
            });

            // [GET] Endpoint lấy báo cáo tổng quan
            // http://localhost:5000/api/v1/ecommerce/reports/overview-report
            v1.MapGet("/overview-report", async (IReportService reportService, [FromQuery] DateTime? date) =>
            {
                var result = await reportService.GetOverviewReportAsyncV1(date);
                // Trả về kết quả nếu có dữ liệu, nếu không trả về mã NotFound (không tìm thấy)
                return result != null ? Results.Ok(result) : Results.NotFound();
            });

            return builder;
        }
    }
}