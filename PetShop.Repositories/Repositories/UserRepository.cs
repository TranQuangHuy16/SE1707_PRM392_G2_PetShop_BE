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
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository() { }

        public UserRepository(PetShopDbContext context) => _context = context;

        public async Task<User> GetUserByUsernameAndPasswordAsync(string username, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetUserByPhoneAsync(string phone)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Phone == phone);
        }

        public async Task<User> CreateUserAsync(User newUser)
        {
            // 1️⃣ Thêm user trước
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync(); // Bắt buộc gọi ở đây để có UserId thật

            // 2️⃣ Lấy admin ID
            var adminId = await _context.Users
                .Where(u => u.Role == Models.Enums.UserRoleEnum.Admin)
                .Select(u => u.UserId)
                .FirstOrDefaultAsync();

            // 3️⃣ Tạo ChatRoom
            var chatRoom = new ChatRoom
            {
                AdminId = adminId,
                CustomerId = newUser.UserId // Lúc này đã có giá trị thật
            };

            await _context.ChatRooms.AddAsync(chatRoom);
            await _context.SaveChangesAsync();

            return newUser;
        }


        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
        }
    }
}
