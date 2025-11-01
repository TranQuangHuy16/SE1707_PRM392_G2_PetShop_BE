using PetShop.Repositories.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.DTOs.Responses
{
    public class PaymentResponse
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public PaymentMethodEnum PaymentMethod { get; set; }
        public string PaymentMethodName { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatusEnum PaymentStatus { get; set; }
        public string PaymentStatusName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
