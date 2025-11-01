using AutoMapper;
using PetShop.API.DTOs;
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
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<UserDetailResponseDto>> GetAll()
        {
            var users = await _userRepository.GetAllUsersAsync();

            var userResponses = users.Select(
                user => _mapper.Map<UserDetailResponseDto>(user)
            );

            return userResponses;
        }

        public async Task<UserDetailResponseDto> GetDetailAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null) return null!;

            var userResponse = _mapper.Map<UserDetailResponseDto>(user);

            return userResponse;
        }

        public async Task<UserResponse> GetById(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null) throw new Exception("User not found");

            var userResponse = _mapper.Map<UserResponse>(user);

            return userResponse;
        }

        public async Task<UserDetailResponseDto> UpdateUserAsync(int id, UpdateUserRequest request)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null) throw new Exception("User not found");

            // Update Role if provided
            if (request.Role.HasValue)
            {
                user.Role = request.Role.Value;
            }

            // Update IsActive if provided
            if (request.IsActive.HasValue)
            {
                user.IsActive = request.IsActive.Value;
            }

            int res = await _userRepository.UpdateAsync(user);

            if (res == 0) throw new Exception("Update failed");

            var userResponse = _mapper.Map<UserDetailResponseDto>(user);

            return userResponse;
        }
        public async Task<int> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null) throw new Exception("User not found");

            user.IsActive = false;

            int res = await _userRepository.UpdateAsync(user);

            if (res == 0) throw new Exception("Delete failed");

            return res;
        }
    }
}

