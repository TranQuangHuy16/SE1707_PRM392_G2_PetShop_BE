using System;
using System.Collections.Generic;

namespace PetShop.Repositories.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        // Navigation
        public User User { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }
}
