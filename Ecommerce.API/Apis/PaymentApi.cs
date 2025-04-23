using Ecommerce.Infrastructure.ExternalServices.Payment.VnPay;

namespace Ecommerce.API.Apis
{
    public static class PaymentApi
    {
        public static IEndpointRouteBuilder MapPaymentApi(this IEndpointRouteBuilder builder)
        {
            var vApi = builder.NewVersionedApi("ecommerce");
            var v1 = vApi.MapGroup("api/v{version:apiVersion}/ecommerce").HasApiVersion(1, 0);

            // Tạo URL thanh toán VNPay
            v1.MapPost("/payment/vnpay/", (
                HttpContext context,
                IVnPayService vnPayService,
                VnPaymentRequestModel model
            ) =>
            {
                var paymentUrl = vnPayService.CreatePaymentUrl(context, model);
                return Results.Ok(new { url = paymentUrl });
            });

            // Xử lý callback sau khi thanh toán
            v1.MapGet("/payment/vnpay-return", async (
                HttpContext context,
                IVnPayService vnPayService,
                IOrderRepository orderRepository
                //EmailService emailService
            ) =>
            {
                var response = vnPayService.ProcessPaymentResponse(context.Request.Query);

                if (response.Success && Guid.TryParse(response.OrderCode, out var orderId))
                {
                    var order = await orderRepository.GetByIdAsync(orderId);
                    if (order != null)
                    {
                        order.Status = OrderStatus.PROCESSING;
                        await orderRepository.UpdateAsync(order);

                        //await emailService.SendEmailAsync(order.CustomerEmail,
                        //    "Hóa đơn thanh toán",
                        //    $"<h2>Thanh toán thành công</h2><p>Đơn hàng: {order.Id}<br/>Tổng tiền: {order.TotalAmount:N0} VND</p>");
                    }
                }

                return Results.Json(response);
            });

            v1.MapPost("/payments/vnpay/verify", async (
                [FromBody] Dictionary<string, string> vnpParams,
                IVnPayService vnPayService) =>
            {
                try
                {
                    var result = await vnPayService.VerifyVnPayPaymentAsync(vnpParams);
                    return Results.Ok(new
                    {
                        success = result.IsSuccess
                    });
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Xác thực thất bại: {ex.Message}");
                }
            });




            return builder;
        }
    }

}
