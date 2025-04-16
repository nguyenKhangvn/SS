using Ecommerce.Infrastructure.ExternalServices.Payment.VnPay;

namespace Ecommerce.API.Services.Interfaces
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel ProcessPaymentResponse(IQueryCollection collections);
    }
}
