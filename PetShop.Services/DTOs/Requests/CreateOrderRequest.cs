using System.ComponentModel.DataAnnotations;

namespace PetShop.Services.DTOs.Requests
{
    public class CreateOrderRequest
    {
        public int? AddressId { get; set; }
        
        public string? Note { get; set; }
    }
}
