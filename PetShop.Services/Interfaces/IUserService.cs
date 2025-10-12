using PetShop.Repositories.Models;
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
    }
}
