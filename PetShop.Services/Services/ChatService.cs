using PetShop.Repositories.Interfaces;
using PetShop.Repositories.Models;
using PetShop.Services.DTOs.Requests;
using PetShop.Services.DTOs.Responses;
using PetShop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;

        public ChatService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task<ChatRoom> CreateChatRoomAsync(CreateChatRoomRequest dto)
        {
            // 🔍 Kiểm tra xem phòng đã tồn tại giữa Customer và Admin chưa
            var existingRoom = await _chatRepository.GetChatRoomByUserAsync(dto.CustomerId, dto.AdminId);
            if (existingRoom != null)
            {
                return existingRoom;
            }

            // 🚀 Tạo phòng mới
            var room = new ChatRoom
            {
                CustomerId = dto.CustomerId,
                AdminId = dto.AdminId,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            var roomId = await _chatRepository.CreateChatRoomAsync(room);
            room.ChatRoomId = roomId;

            return room;
        }

        public async Task<ChatRoom?> GetChatRoomByIdAsync(int customerId)
        {
            return await _chatRepository.GetChatRoomByIdAsync(customerId);
        }

        public async Task<List<Message>> GetMessagesAsync(int chatRoomId)
        {
            return await _chatRepository.GetMessagesAsync(chatRoomId);
        }

        public async Task<int> SendMessageAsync(ReceiveMessageResponse response)
        {
            var message = new Message()
            {
                SenderId = response.SenderId,
                ChatRoomId = response.ChatRoomId,
                MessageText = response.MessageText,
                SentAt = DateTime.Now
            };
            return await _chatRepository.SendMessageAsync(message);
        }

        public async Task<bool> DeleteChatRoomAsync(int id)
        {
            return await _chatRepository.DeleteChatRoomAsync(id);
        }
    }
}
