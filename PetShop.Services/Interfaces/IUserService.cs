using PetShop.API.DTOs;
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
    public interface IUserService
    {
        Task<UserResponse> GetById(int id);
        Task<IEnumerable<UserDetailResponseDto>> GetAll();
        Task<UserDetailResponseDto> GetDetailAsync(int id);
        Task<UserDetailResponseDto> UpdateUserAsync(int id, UpdateUserRequest request);
        Task<int> DeleteUserAsync(int id);
    }
}
