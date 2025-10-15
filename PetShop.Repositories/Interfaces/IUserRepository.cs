using PetShop.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> CreateUserAsync(User newUser);
        Task<User> GetUserByUsernameAndPasswordAsync(string username, string password);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByPhoneAsync(string phone);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByIdAsync(int id);
        Task<int> UpdateAsync(User user);
    }
}
