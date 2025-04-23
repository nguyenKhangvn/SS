using Ecommerce.Infrastructure.ExternalServices.Payment.VnPay;

namespace Ecommerce.API.Services.Interfaces
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel ProcessPaymentResponse(IQueryCollection collections);
        // Xử lý phản hồi từ VNPay
        Task<PaymentVerificationResult> VerifyVnPayPaymentAsync(Dictionary<string, string> vnpParams);

    }
}
