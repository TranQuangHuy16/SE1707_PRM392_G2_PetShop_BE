using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.DTOs.Requests
{
    public class ProductUpdateRequest
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        public string? Description { get; set; }

        [Required]
        [Range(0.01, (double)decimal.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
