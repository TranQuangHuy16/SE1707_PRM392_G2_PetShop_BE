using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendOtpAsync(string toEmail, string subject, string body);
    }
}
