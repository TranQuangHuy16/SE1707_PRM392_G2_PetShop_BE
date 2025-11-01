using PetShop.Repositories.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.DTOs.Requests
{
    public class UpdateUserRequest
    {
        public UserRoleEnum? Role { get; set; }
        public bool? IsActive { get; set; }
    }
}
