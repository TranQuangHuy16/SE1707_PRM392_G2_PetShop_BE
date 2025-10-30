using PetShop.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Payment>> GetAllPaymentsAsync();
        Task<Payment?> GetPaymentByIdAsync(int paymentId);
        Task<Payment?> GetPaymentByOrderIdAsync(int orderId);
        Task<IEnumerable<Payment>> GetPaymentsByUserIdAsync(int userId);
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task UpdatePaymentAsync(Payment payment);
        Task DeletePaymentAsync(Payment payment);
    }
}
