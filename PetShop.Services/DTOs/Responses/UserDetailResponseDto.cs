using PetShop.Repositories.Models;

namespace PetShop.API.DTOs
{
    public class UserDetailResponseDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? ImgUrl { get; set; }
        public int Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<UserAddress> Addresses { get; set; }
    }
}
