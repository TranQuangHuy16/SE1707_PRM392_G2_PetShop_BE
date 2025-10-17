using Firebase.Database;
using Firebase.Database.Query;
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
    public class ChatRepository : IChatRepository
    {
        private readonly PetShopDbContext _context;

        public ChatRepository(PetShopDbContext context)
        {
            _context = context;
        }

        public async Task<ChatRoom?> GetChatRoomByUserAsync(int customerId, int adminId)
        {
            return await _context.ChatRooms
                .FirstOrDefaultAsync(r => r.CustomerId == customerId && r.AdminId == adminId);
        }

        public async Task<int> CreateChatRoomAsync(ChatRoom room)
        {
            _context.ChatRooms.Add(room);
            await _context.SaveChangesAsync();
            return room.ChatRoomId;
        }

        public async Task<bool> DeleteChatRoomAsync(int id)
        {
            var room = await _context.ChatRooms.FindAsync(id);
            if (room == null) return false;

            _context.ChatRooms.Remove(room);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ChatRoom?> GetChatRoomByIdAsync(int customerId)
        {
            return await _context.ChatRooms
                .Include(r => r.Messages)
                .FirstOrDefaultAsync(r => r.CustomerId == customerId);
        }

        public async Task<int> SendMessageAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message.MessageId;
        }

        public async Task<List<Message>> GetMessagesAsync(int chatRoomId)
        {
            return await _context.Messages
                .Where(m => m.ChatRoomId == chatRoomId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }
    }
}

