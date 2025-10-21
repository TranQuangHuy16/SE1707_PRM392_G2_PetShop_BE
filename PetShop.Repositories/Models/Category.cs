using System.Collections.Generic;

namespace PetShop.Repositories.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool IsActive { get; set; } = true;

        public string? ImageUrl { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
