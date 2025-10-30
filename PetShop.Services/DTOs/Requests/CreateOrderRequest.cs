using System.ComponentModel.DataAnnotations;

namespace PetShop.Services.DTOs.Requests
{
    public class CreateOrderRequest
    {
        public int? AddressId { get; set; }
        
        [Required]
        public string? Note { get; set; }
    }
}
