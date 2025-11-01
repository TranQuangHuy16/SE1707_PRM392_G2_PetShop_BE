using PetShop.Repositories.Models.Enums;
using PetShop.Services.DTOs.Requests;
using PetShop.Services.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentResponse>> GetAllPaymentsAsync(PaymentStatusEnum? status = null);
        Task<PaymentResponse?> GetPaymentByIdAsync(int paymentId);
        Task<PaymentResponse?> GetPaymentByOrderIdAsync(int orderId);
        Task<IEnumerable<PaymentResponse>> GetPaymentsByUserIdAsync(int userId, PaymentStatusEnum? status = null);
        Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request);
        Task<bool> UpdatePaymentStatusAsync(int paymentId, UpdatePaymentStatusRequest request);
        Task<bool> DeletePaymentAsync(int paymentId);
        Task<Dictionary<string, object>> ProcessZaloPayCallbackAsync(Dictionary<string, object> cbdata);
        Task<bool> ConfirmPaymentByOrderIdAsync(int orderId);
    }
}
