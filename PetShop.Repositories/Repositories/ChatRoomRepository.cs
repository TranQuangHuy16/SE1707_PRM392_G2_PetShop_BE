using PetShop.Repositories.Basic;
using PetShop.Repositories.DBContext;
using PetShop.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Repositories.Repositories
{
    public class ChatRoomRepository : GenericRepository<ChatRoom>
    {
        public ChatRoomRepository() { }

        public ChatRoomRepository(PetShopDbContext context) => _context = context;

        public async Task<ChatRoom> GetChatRoomByUsersAsync(int userId1, int userId2)
        {
            return _context.ChatRooms
                .FirstOrDefault(cr => cr.AdminId == userId1 && cr.CustomerId == userId2
                                   || cr.AdminId == userId2 && cr.CustomerId == userId1);
        }

        public async Task<ChatRoom> CreateChatRoomAsync(ChatRoom newChatRoom)
        {
            await _context.ChatRooms.AddAsync(newChatRoom);
            await _context.SaveChangesAsync();

            return newChatRoom;
        }
    }
}
