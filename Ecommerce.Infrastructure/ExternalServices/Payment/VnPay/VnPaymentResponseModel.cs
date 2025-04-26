using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.ExternalServices.Payment.VnPay
{
    public class VnPaymentResponseModel
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? OrderDescription { get; set; }
        public string? OrderCode { get; set; }
        public string? TransactionId { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Amount { get; set; }
        public string? BankCode { get; set; }
        public string? BankTranNo { get; set; }
        public string? CardType { get; set; }
        public string? PayDate { get; set; }
        public string? TmnCode { get; set; }
        public string? TransactionStatus { get; set; }
        public string? VnPayResponseCode { get; set; }
        public string? Token { get; set; }
    }

    public class VnPaymentRequestModel
    {
        public string OrderCode { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class PaymentVerificationResult
    {
        public bool IsSuccess { get; set; }
    }

    public class VnPayCallbackRequest
    {
        public string vnp_TxnRef { get; set; } = string.Empty;
        public string vnp_ResponseCode { get; set; } = string.Empty;
        public string vnp_TransactionNo { get; set; } = string.Empty;
        public string vnp_CardType { get; set; } = string.Empty;
        public string vnp_OrderInfo { get; set; } = string.Empty;
        // Thêm các trường khác nếu cần
    }

}
