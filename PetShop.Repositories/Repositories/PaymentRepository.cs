using Microsoft.EntityFrameworkCore;
using PetShop.Repositories.DBContext;
using PetShop.Repositories.Interfaces;
using PetShop.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Repositories.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PetShopDbContext _context;

        public PaymentRepository(PetShopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            return await _context.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o.User)
                .Where(p => p.IsActive)
                .ToListAsync();
        }

        public async Task<Payment?> GetPaymentByIdAsync(int paymentId)
        {
            return await _context.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o.User)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId && p.IsActive);
        }

        public async Task<Payment?> GetPaymentByOrderIdAsync(int orderId)
        {
            return await _context.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o.User)
                .FirstOrDefaultAsync(p => p.OrderId == orderId && p.IsActive);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByUserIdAsync(int userId)
        {
            return await _context.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o.User)
                .Where(p => p.Order.UserId == userId && p.IsActive)
                .ToListAsync();
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task UpdatePaymentAsync(Payment payment)
        {
            _context.Entry(payment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeletePaymentAsync(Payment payment)
        {
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
        }
    }
}
