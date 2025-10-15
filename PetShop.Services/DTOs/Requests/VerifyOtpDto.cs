using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.DTOs.Requests
{
    public class VerifyOtpDto
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }
}
