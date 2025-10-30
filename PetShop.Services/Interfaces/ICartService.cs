using PetShop.Services.DTOs.Requests;
using PetShop.Services.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartResponse?> GetCartByUserIdAsync(int userId);
        Task<CartResponse> AddToCartAsync(int userId, AddToCartRequest request);
        Task<bool> UpdateCartItemAsync(int userId, int cartItemId, UpdateCartItemRequest request);
        Task<bool> RemoveCartItemAsync(int userId, int cartItemId);
        Task<bool> ClearCartAsync(int userId);
    }
}
