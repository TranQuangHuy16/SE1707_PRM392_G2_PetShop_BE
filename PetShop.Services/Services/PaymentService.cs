using AutoMapper;
using PetShop.Repositories.Interfaces;
using PetShop.Repositories.Models;
using PetShop.Repositories.Models.Enums;
using PetShop.Services.DTOs.Requests;
using PetShop.Services.DTOs.Responses;
using PetShop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PetShop.Services.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IMapper _mapper;

        public PaymentService(IPaymentRepository paymentRepo, IMapper mapper)
        {
            _paymentRepo = paymentRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PaymentResponse>> GetAllPaymentsAsync(PaymentStatusEnum? status = null)
        {
            var payments = await _paymentRepo.GetAllPaymentsAsync(status);
            return _mapper.Map<IEnumerable<PaymentResponse>>(payments);
        }

        public async Task<PaymentResponse?> GetPaymentByIdAsync(int paymentId)
        {
            var payment = await _paymentRepo.GetPaymentByIdAsync(paymentId);
            if (payment == null)
            {
                return null;
            }
            return _mapper.Map<PaymentResponse>(payment);
        }

        public async Task<PaymentResponse?> GetPaymentByOrderIdAsync(int orderId)
        {
            var payment = await _paymentRepo.GetPaymentByOrderIdAsync(orderId);
            if (payment == null)
            {
                return null;
            }
            return _mapper.Map<PaymentResponse>(payment);
        }

        public async Task<IEnumerable<PaymentResponse>> GetPaymentsByUserIdAsync(int userId, PaymentStatusEnum? status = null)
        {
            var payments = await _paymentRepo.GetPaymentsByUserIdAsync(userId, status);
            return _mapper.Map<IEnumerable<PaymentResponse>>(payments);
        }

        public async Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request)
        {
            var payment = _mapper.Map<Payment>(request);
            payment.PaymentDate = DateTime.Now;
            
            var newPayment = await _paymentRepo.CreatePaymentAsync(payment);
            return _mapper.Map<PaymentResponse>(newPayment);
        }

        public async Task<bool> UpdatePaymentStatusAsync(int paymentId, UpdatePaymentStatusRequest request)
        {
            var existingPayment = await _paymentRepo.GetPaymentByIdAsync(paymentId);
            if (existingPayment == null)
            {
                return false;
            }

            existingPayment.PaymentStatus = request.PaymentStatus;
            await _paymentRepo.UpdatePaymentAsync(existingPayment);
            return true;
        }

        public async Task<bool> DeletePaymentAsync(int paymentId)
        {
            var payment = await _paymentRepo.GetPaymentByIdAsync(paymentId);
            if (payment == null)
            {
                return false;
            }

            await _paymentRepo.DeletePaymentAsync(payment);
            return true;
        }

        public async Task<Dictionary<string, object>> ProcessZaloPayCallbackAsync(Dictionary<string, object> cbdata)
        {
            var result = new Dictionary<string, object>();

            try
            {
                // Lấy dữ liệu từ callback
                string dataStr = cbdata["data"]?.ToString() ?? "";
                string reqMac = cbdata["mac"]?.ToString() ?? "";

                // Verify MAC signature
                string key2 = "kLtgPl8HHhfvMuDHPwKfgfsY4Ydm9eIz"; // ZaloPay Key2 - phải khớp với config
                string mac = ComputeHmacSha256(dataStr, key2);

                // Kiểm tra MAC
                if (!mac.Equals(reqMac))
                {
                    result["return_code"] = -1;
                    result["return_message"] = "mac not equal";
                    return result;
                }

                // Parse data JSON
                var callbackData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(dataStr);
                if (callbackData == null)
                {
                    result["return_code"] = 0;
                    result["return_message"] = "Invalid callback data";
                    return result;
                }

                // Lấy app_trans_id từ callback data
                string appTransId = callbackData["app_trans_id"].GetString() ?? "";
                
                // Extract OrderId từ app_trans_id format: yyMMdd_OrderId
                string[] parts = appTransId.Split('_');
                if (parts.Length < 2 || !int.TryParse(parts[1], out int orderId))
                {
                    result["return_code"] = 0;
                    result["return_message"] = "Invalid app_trans_id format";
                    return result;
                }

                // Tìm payment theo OrderId
                var payment = await _paymentRepo.GetPaymentByOrderIdAsync(orderId);
                if (payment == null)
                {
                    result["return_code"] = 0;
                    result["return_message"] = "Payment not found";
                    return result;
                }

                // Cập nhật payment status
                payment.PaymentStatus = PaymentStatusEnum.Success;
                payment.PaymentDate = DateTime.Now;
                await _paymentRepo.UpdatePaymentAsync(payment);

                result["return_code"] = 1;
                result["return_message"] = "success";
            }
            catch (Exception ex)
            {
                result["return_code"] = 0;
                result["return_message"] = ex.Message;
            }

            return result;
        }

        public async Task<bool> ConfirmPaymentByOrderIdAsync(int orderId)
        {
            try
            {
                var payment = await _paymentRepo.GetPaymentByOrderIdAsync(orderId);
                
                if (payment == null)
                {
                    return false;
                }

                if (payment.PaymentStatus == PaymentStatusEnum.Success)
                {
                    return true;
                }

                payment.PaymentStatus = PaymentStatusEnum.Success;
                payment.PaymentDate = DateTime.Now;
                await _paymentRepo.UpdatePaymentAsync(payment);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string ComputeHmacSha256(string message, string key)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(messageBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
