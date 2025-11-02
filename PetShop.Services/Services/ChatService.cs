using PetShop.Repositories.Interfaces;
using PetShop.Repositories.Models;
using PetShop.Repositories.Repositories;
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
        private readonly IUserRepository _userRepository;
        private readonly NotificationService _notificationService;

        public ChatService(IChatRepository chatRepository, IUserRepository userRepository, NotificationService notificationService)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
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
            // 1️⃣ Lưu message vào database thông qua repository
            var message = new Message
            {
                ChatRoomId = response.ChatRoomId,
                SenderId = response.SenderId,
                MessageText = response.MessageText,
                SentAt = DateTime.UtcNow
            };



            // 2️⃣ Xác định người nhận
            var room = await _chatRepository.GetChatRoomByRoomIdAsync(response.ChatRoomId);
            if (room == null)
                throw new Exception("Chat room not found");

            int receiverId = (room.CustomerId == response.SenderId)
                ? room.AdminId
                : room.CustomerId;

            var receiver = await _userRepository.GetUserByIdAsync(receiverId);

            // 3️⃣ Gửi thông báo qua Firebase
            if (!string.IsNullOrEmpty(receiver?.FcmToken))
            {
                await _notificationService.SendMessageAsync(
                    receiver.FcmToken,
                    "New Message",
                    response.MessageText
                );
            }

            return await _chatRepository.SendMessageAsync(message);
        }

        public async Task<bool> DeleteChatRoomAsync(int id)
        {
            return await _chatRepository.DeleteChatRoomAsync(id);
        }

        public async Task<IEnumerable<ChatRoom>> GetChatRoomByAdminIdAsync(int adminId)
        {
            return await _chatRepository.GetChatRoomByAdminIdAsync(adminId);
        }
    }
}
