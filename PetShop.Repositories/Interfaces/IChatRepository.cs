using PetShop.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Repositories.Interfaces
{
    public interface IChatRepository
    {
        Task<ChatRoom?> GetChatRoomByUserAsync(int customerId, int adminId);
        Task<int> CreateChatRoomAsync(ChatRoom room);
        Task<bool> DeleteChatRoomAsync(int id);
        Task<ChatRoom?> GetChatRoomByIdAsync(int id);

        Task<int> SendMessageAsync(Message message);
        Task<List<Message>> GetMessagesAsync(int chatRoomId);
    }
}
