using System;
using System.ComponentModel.DataAnnotations;

namespace PetShop.Repositories.Models
{
    public class UserAddress
    {
        public int AddressId { get; set; }
        public int UserId { get; set; }
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string PostalCode { get; set; }
        public bool IsDefault { get; set; } = false;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Navigation
        public User User { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
