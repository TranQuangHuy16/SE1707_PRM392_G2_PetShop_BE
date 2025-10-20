using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.DTOs.Requests
{
    public class CategoryUpdateRequest
    {
        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
    }
}
