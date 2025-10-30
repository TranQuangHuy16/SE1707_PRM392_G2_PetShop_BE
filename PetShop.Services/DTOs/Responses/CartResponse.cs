using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.DTOs.Responses
{
    public class CartResponse
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public ICollection<CartItemResponse> CartItems { get; set; } = new List<CartItemResponse>();
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
    }
}
