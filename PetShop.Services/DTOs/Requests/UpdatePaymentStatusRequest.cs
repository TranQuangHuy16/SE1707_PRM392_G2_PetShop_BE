using PetShop.Repositories.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.DTOs.Requests
{
    public class UpdatePaymentStatusRequest
    {
        [Required]
        public PaymentStatusEnum PaymentStatus { get; set; }
    }
}
