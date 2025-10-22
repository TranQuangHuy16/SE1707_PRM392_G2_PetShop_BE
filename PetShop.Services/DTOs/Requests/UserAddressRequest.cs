using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.DTOs.Requests
{
    public class UserAddressRequest
    {
        public int UserId { get; set; }
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string PostalCode { get; set; }
        public bool IsDefault { get; set; } = false;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
