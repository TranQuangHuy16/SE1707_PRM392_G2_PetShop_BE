using PetShop.Repositories.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Repositories.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Phone { get; set; }
        public UserRoleEnum Role { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }

        // Navigation
        public ICollection<UserAddress> Addresses { get; set; }
        public ICollection<Cart> Carts { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<ChatRoom> CustomerChatRooms { get; set; }
        public ICollection<ChatRoom> AdminChatRooms { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<Otp> Otps { get; set; } = new List<Otp>();
    }
}
