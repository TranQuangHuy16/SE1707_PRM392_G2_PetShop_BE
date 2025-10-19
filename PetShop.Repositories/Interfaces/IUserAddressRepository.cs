using PetShop.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Repositories.Interfaces
{
    public interface IUserAddressRepository
    {
        Task<int> CreateAsync(UserAddress userAddress);
        Task<int> UpdateAsync(UserAddress userAddress);
        Task<UserAddress> GetDefaultByUserId(int userId);
        Task<UserAddress> GetByIdAsync(int id);
        Task<IEnumerable<UserAddress>> GetByUserIdAsync(int userId);
    }
}
