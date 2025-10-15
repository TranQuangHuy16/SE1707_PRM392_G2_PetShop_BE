using Microsoft.EntityFrameworkCore;
using PetShop.Repositories.Basic;
using PetShop.Repositories.DBContext;
using PetShop.Repositories.Interfaces;
using PetShop.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Repositories.Repositories
{
    public class OtpRepository : GenericRepository<Otp>, IOtpRepository
    {
        public OtpRepository() { }

        public OtpRepository(PetShopDbContext context) => _context = context;
        public async Task DeleteExpiredAsync(DateTime now)
        {
            var expired = await _context.Otps
            .Where(o => o.ExpiresAt < now)
            .ToListAsync();

            if (expired.Any())
                _context.Otps.RemoveRange(expired);

            _context.SaveChangesAsync();
        }

        public async Task DeleteUsedAsync()
        {
            var usedOtps = await _context.Otps
            .Where(o => o.IsUsed)
            .ToListAsync();

            if (usedOtps.Any())
                _context.Otps.RemoveRange(usedOtps);
            _context.SaveChangesAsync();
        }

        public async Task<Otp?> GetValidOtpByCodeAsync(int userId, string code)
        {
            var now = DateTime.UtcNow;
            return await _context.Otps
                .Where(o => o.UserId == userId && o.Code == code && !o.IsUsed && o.ExpiresAt >= now)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
        }
        //public Task MarkUsedAsync(Otp otp)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
