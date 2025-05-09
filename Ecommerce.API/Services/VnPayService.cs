﻿using Ecommerce.Infrastructure.Entity;
using Ecommerce.Infrastructure.ExternalServices.Payment.VnPay;

namespace Ecommerce.API.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _config;

        public VnPayService(IConfiguration config)
        {
            _config = config;
        }
        public string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model)
        {
            var tick = DateTime.Now.Ticks.ToString();
            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"]);
            vnpay.AddRequestData("vnp_Command", _config["VnPay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString()); //Số tiền thanh toán. Số tiền không 
            //mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND
            //(một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần(khử phần thập phân), sau đó gửi sang VNPAY

            vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_ExpireDate", model.CreatedDate.AddMinutes(15).ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", _config["VnPay:Locale"]);
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang: " + model.OrderCode);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", _config["VnPay:ReturnUrl"]);
            //vnpay.AddRequestData("vnp_TxnRef", tick); // Mã tham chiếu của giao dịch tại hệ 
                                                      //thống của merchant.Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY.Không được
                                                      //trùng lặp trong ngày
            vnpay.AddRequestData("vnp_TxnRef", model.OrderCode);
            //var hash = _config["VnPay:HashSecret"];
            var payUrl = vnpay.CreateRequestUrl(_config["VnPay:BaseUrl"], _config["VnPay:HashSecret"]);
            return payUrl;
        }

        public VnPaymentResponseModel ProcessPaymentResponse(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }
            var vnp_Amount = vnpay.GetResponseData("vnp_Amount");
            var vnp_BankCode = vnpay.GetResponseData("vnp_BankCode");
            var vnp_BankTranNo = vnpay.GetResponseData("vnp_BankTranNo");
            var vnp_CardType = vnpay.GetResponseData("vnp_CardType");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
            var vnp_PayDate = vnpay.GetResponseData("vnp_PayDate");
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
            var vnp_TmnCode = vnpay.GetResponseData("vnp_TmnCode");
            var vnp_TxnRef = vnpay.GetResponseData("vnp_TxnRef");
            var vnp_TransactionNo = vnpay.GetResponseData("vnp_TransactionNo");
            var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config["VnPay:HashSecret"]);
            if (!checkSignature)
            {
                return new VnPaymentResponseModel
                {
                    Success = false
                };
            }

            return new VnPaymentResponseModel
            {
                Success = true,
                Message = "Thanh toán thành công",
                PaymentMethod = "VnPay",
                OrderDescription = vnp_OrderInfo,
                OrderCode = vnp_TxnRef,
                TransactionId = vnp_TransactionNo,
                Amount = vnp_Amount,
                BankCode = vnp_BankCode,
                BankTranNo = vnp_BankTranNo,
                CardType = vnp_CardType,
                PayDate = vnp_PayDate,
                TmnCode = vnp_TmnCode,
                TransactionStatus = vnp_TransactionStatus,
                VnPayResponseCode = vnp_ResponseCode,
                Token = vnp_SecureHash
            };
        }
        public async Task<PaymentVerificationResult> VerifyVnPayPaymentAsync(Dictionary<string, string> vnpParams)
        {
            // TODO: lấy các thông tin cần thiết để xác thực chữ ký (signature)
            string vnp_SecureHash = vnpParams["vnp_SecureHash"];
            vnpParams.Remove("vnp_SecureHash");
            vnpParams.Remove("vnp_SecureHashType");

            // Sắp xếp lại key theo thứ tự alphabet
            var sortedParams = vnpParams.OrderBy(x => x.Key);

            var signData = string.Join("&", sortedParams.Select(kv => $"{kv.Key}={kv.Value}"));
            var hashSecret = _config["VnPay:HashSecret"]; // lấy từ appsettings.json

            string computedHash = Utils.HmacSHA512(hashSecret, signData);

            bool isSuccess = vnp_SecureHash.Equals(computedHash, StringComparison.OrdinalIgnoreCase)
                             && vnpParams["vnp_ResponseCode"] == "00";

            return new PaymentVerificationResult
            {
                IsSuccess = isSuccess
            };
        }
    }
}
