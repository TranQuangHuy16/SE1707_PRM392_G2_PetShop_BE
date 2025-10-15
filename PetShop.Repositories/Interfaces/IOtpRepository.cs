using PetShop.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Repositories.Interfaces
{
    public interface IOtpRepository
    {
        Task<Otp?> GetValidOtpByCodeAsync(int userId, string code);
        Task DeleteExpiredAsync(DateTime now);
        Task DeleteUsedAsync();
        Task<int> CreateAsync(Otp otp);
        Task<int> UpdateAsync(Otp otp);
    }
}
