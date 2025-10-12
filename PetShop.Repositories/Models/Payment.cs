using PetShop.Repositories.Models.Enums;
using System;

namespace PetShop.Repositories.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public PaymentMethodEnum PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public decimal Amount { get; set; }
        public PaymentStatusEnum PaymentStatus { get; set; } = PaymentStatusEnum.Pending;
        public bool IsActive { get; set; } = true;

        // Navigation
        public Order Order { get; set; }
    }
}
