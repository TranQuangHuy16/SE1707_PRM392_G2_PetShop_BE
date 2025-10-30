using AutoMapper;
using PetShop.Repositories.Interfaces;
using PetShop.Repositories.Models;
using PetShop.Services.DTOs.Requests;
using PetShop.Services.DTOs.Responses;
using PetShop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<IEnumerable<PaymentResponse>> GetAllPaymentsAsync()
        {
            var payments = await _paymentRepo.GetAllPaymentsAsync();
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

        public async Task<IEnumerable<PaymentResponse>> GetPaymentsByUserIdAsync(int userId)
        {
            var payments = await _paymentRepo.GetPaymentsByUserIdAsync(userId);
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
    }
}
