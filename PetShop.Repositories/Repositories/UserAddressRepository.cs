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
    public class UserAddressRepository : GenericRepository<UserAddress>, IUserAddressRepository
    {
        public UserAddressRepository() { }
        public UserAddressRepository(PetShopDbContext context) => _context = context;

        public async Task<IEnumerable<UserAddress>> GetByUserIdAsync(int userId)
        {
            return await _context.UserAddresses.Where(u => u.UserId == userId).ToListAsync();
        }

        public async Task<UserAddress> GetDefaultByUserId(int userId)
        {
            return await _context.UserAddresses
                .FirstOrDefaultAsync(u => u.UserId == userId && u.IsDefault == true);
        }
    }
}
