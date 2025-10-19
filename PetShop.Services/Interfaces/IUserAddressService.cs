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
    public interface IUserAddressService
    {
        Task<UserAddressResponse> CreateAsync(UserAddressRequest request);
        Task<UserAddressResponse> UpdateAsync(int id, UserAddressRequest request);
        Task<UserAddressResponse> GetDefaultByUserId(int id);
    }
}
