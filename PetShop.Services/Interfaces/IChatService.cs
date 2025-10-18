using PetShop.Repositories.Models;
using PetShop.Services.DTOs.Requests;
using PetShop.Services.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.Interfaces
{
    public interface IChatService
    {
        Task<ChatRoom> CreateChatRoomAsync(CreateChatRoomRequest dto);
        Task<ChatRoom?> GetChatRoomByIdAsync(int id);
        Task<List<Message>> GetMessagesAsync(int chatRoomId);
        Task<int> SendMessageAsync(ReceiveMessageResponse response);
        Task<bool> DeleteChatRoomAsync(int id);
    }
}
