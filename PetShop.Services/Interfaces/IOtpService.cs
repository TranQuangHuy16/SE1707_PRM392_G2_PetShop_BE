using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.Interfaces
{
    public interface IOtpService
    {
        Task<bool> RequestOtpAsync(string email);
        Task<bool> VerifyOtpAsync(string email, string code);
        Task<bool> ResetPasswordAsync(string email, string code, string newPassword);
    }
}
