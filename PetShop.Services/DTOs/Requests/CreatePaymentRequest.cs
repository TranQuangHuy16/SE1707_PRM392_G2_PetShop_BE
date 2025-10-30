using PetShop.Repositories.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.DTOs.Requests
{
    public class CreatePaymentRequest
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public PaymentMethodEnum PaymentMethod { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
    }
}
