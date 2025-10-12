using System.Collections.Generic;

namespace PetShop.Repositories.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string ProductName { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation
        public Category Category { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
