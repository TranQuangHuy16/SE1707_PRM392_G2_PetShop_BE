namespace PetShop.Services
{
    public class UpdateUserDetailsRequest
    {
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? ImgUrl { get; set; }
    }
}