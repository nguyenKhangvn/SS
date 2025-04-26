using Ecommerce.Infrastructure.ExternalServices.Payment.VnPay;
using Ecommerce.Infrastructure.Models.Dtos;

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
            v1.MapGet("/payment/vnpay-return", async (HttpContext context) =>
            {
                // Resolve service từ DI
                var vnPayService = context.RequestServices.GetRequiredService<IVnPayService>();
                var orderRepository = context.RequestServices.GetRequiredService<IOrderRepository>();
                var paymentService = context.RequestServices.GetRequiredService<IPaymentService>();
                var response = vnPayService.ProcessPaymentResponse(context.Request.Query);

                if (response?.VnPayResponseCode == "00")
                {
                    var order = await orderRepository.GetOrderByOrderCode(response?.OrderCode);
                    if (order != null)
                    {
                        var paymentDto = new PaymentDto
                        {
                            OrderId = order.Id,
                            OrderCode = order.OrderCode,
                            Amount = order.TotalAmount,
                            PaymentMethod = response.PaymentMethod,
                            Status = PaymentStatus.COMPLETED,
                            TransactionId = response.TransactionId,
                            PaidAt = DateTime.UtcNow
                        };
                        await paymentService.CreateAsync(paymentDto);
                        order.Status = OrderStatus.PROCESSING;
                        await orderRepository.UpdateAsync(order);
                    }
                }

                return Results.Json(response);
            });

            v1.MapGet("/payment/{orderId:guid}", async (Guid orderId, IPaymentService paymentService) =>
            {
                var payment = await paymentService.GetByOrderIdAsync(orderId);
                return payment == null ? Results.NotFound() : Results.Ok(payment);
            });

            v1.MapGet("/payment/", async (IPaymentService paymentService) =>
            {
                var payment = await paymentService.GetAllAsync();
                return payment == null ? Results.NotFound() : Results.Ok(payment);
            });


            return builder;
        }
    }

}
