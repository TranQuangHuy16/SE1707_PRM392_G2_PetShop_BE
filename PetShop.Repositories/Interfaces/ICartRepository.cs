using PetShop.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByUserIdAsync(int userId);
        Task<Cart?> GetCartByIdAsync(int cartId);
        Task<Cart> CreateCartAsync(Cart cart);
        Task UpdateCartAsync(Cart cart);
        Task DeleteCartAsync(Cart cart);
        
        // CartItem methods
        Task<CartItem?> GetCartItemByIdAsync(int cartItemId);
        Task<CartItem?> GetCartItemByCartAndProductAsync(int cartId, int productId);
        Task<IEnumerable<CartItem>> GetCartItemsByCartIdAsync(int cartId);
        Task<CartItem> AddCartItemAsync(CartItem cartItem);
        Task UpdateCartItemAsync(CartItem cartItem);
        Task DeleteCartItemAsync(CartItem cartItem);
        Task ClearCartAsync(int cartId);
    }
}
