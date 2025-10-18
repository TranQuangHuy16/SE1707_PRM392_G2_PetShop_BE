using PetShop.Repositories.Interfaces;
using PetShop.Repositories.Models;
using PetShop.Services.DTOs.Responses;
using PetShop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserResponse> GetById(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null) throw new Exception("User not found");

            var userResponse = new UserResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Password = user.Password,
                Email = user.Email,
                Phone = user.Phone,
                ImgUrl = user.ImgUrl,
            };

            return userResponse;
        }
    }
}
