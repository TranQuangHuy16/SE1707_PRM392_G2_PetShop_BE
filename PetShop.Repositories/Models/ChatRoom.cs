using System;
using System.Collections.Generic;

namespace PetShop.Repositories.Models
{
    public class ChatRoom
    {
        public int ChatRoomId { get; set; }
        public int CustomerId { get; set; }
        public int AdminId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        // Navigation
        public User Customer { get; set; }
        public User Admin { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
