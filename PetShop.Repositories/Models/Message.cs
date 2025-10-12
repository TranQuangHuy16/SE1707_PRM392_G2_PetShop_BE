using System;

namespace PetShop.Repositories.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        public int ChatRoomId { get; set; }
        public int SenderId { get; set; }
        public string? MessageText { get; set; }
        public string? ImageUrl { get; set; }
        public string? FileUrl { get; set; }
        public DateTime SentAt { get; set; } = DateTime.Now;

        // Navigation
        public ChatRoom ChatRoom { get; set; }
        public User Sender { get; set; }
    }
}
