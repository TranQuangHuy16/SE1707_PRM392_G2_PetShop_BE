using Microsoft.EntityFrameworkCore;
using PetShop.Repositories.Basic;
using PetShop.Repositories.DBContext;
using PetShop.Repositories.Interfaces;
using PetShop.Repositories.Models;

namespace PetShop.Repositories.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository() { }

        public UserRepository(PetShopDbContext context) => _context = context;

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                        .Include(add => add.Addresses)
                        .Include(cart => cart.Carts)
                        .Include(order => order.Orders)
                        .Include(notif => notif.Notifications)
                        .Include(cr => cr.CustomerChatRooms)
                        .Include(cr => cr.AdminChatRooms)
                        .Include(msg => msg.Messages)
                        .Include(otp => otp.Otps)
                        .ToListAsync();
        }

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

            // 3️⃣ Chỉ tạo ChatRoom nếu có admin trong hệ thống
            if (adminId > 0)
            {
                var chatRoom = new ChatRoom
                {
                    AdminId = adminId,
                    CustomerId = newUser.UserId // Lúc này đã có giá trị thật
                };

                await _context.ChatRooms.AddAsync(chatRoom);
                await _context.SaveChangesAsync();
            }

            return newUser;
        }


        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users
            .Include(add => add.Addresses)
            .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<int> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            return await _context.SaveChangesAsync();
        }

    }
}
