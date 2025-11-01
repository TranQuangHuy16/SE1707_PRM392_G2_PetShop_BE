using AutoMapper;
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
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepo;
        private readonly IProductRepository _productRepo;
        private readonly IMapper _mapper;

        public CartService(ICartRepository cartRepo, IProductRepository productRepo, IMapper mapper)
        {
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            _mapper = mapper;
        }

        public async Task<CartResponse?> GetCartByUserIdAsync(int userId)
        {
            var cart = await _cartRepo.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return null;
            }

            var response = _mapper.Map<CartResponse>(cart);
            response.TotalAmount = cart.CartItems.Sum(ci => ci.Product.Price * ci.Quantity);
            response.TotalItems = cart.CartItems.Sum(ci => ci.Quantity);

            return response;
        }

        public async Task<CartResponse> AddToCartAsync(int userId, AddToCartRequest request)
        {
            // Check if product exists
            var product = await _productRepo.GetProductByIdAsync(request.ProductId);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            if (!product.IsActive)
            {
                throw new Exception("Product is not available");
            }

            if (product.Stock < request.Quantity)
            {
                throw new Exception($"Insufficient stock. Only {product.Stock} items available");
            }

            // Get or create cart
            var cart = await _cartRepo.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };
                cart = await _cartRepo.CreateCartAsync(cart);
            }

            // Check if item already exists in cart
            var existingCartItem = await _cartRepo.GetCartItemByCartAndProductAsync(cart.CartId, request.ProductId);
            if (existingCartItem != null)
            {
                // Update quantity
                existingCartItem.Quantity += request.Quantity;

                if (product.Stock < existingCartItem.Quantity)
                {
                    throw new Exception($"Insufficient stock. Only {product.Stock} items available");
                }

                await _cartRepo.UpdateCartItemAsync(existingCartItem);
            }
            else
            {
                // Add new cart item
                var cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                };
                await _cartRepo.AddCartItemAsync(cartItem);
            }

            // Return updated cart
            return await GetCartByUserIdAsync(userId) ?? new CartResponse();
        }

        public async Task<bool> UpdateCartItemAsync(int userId, int cartItemId, UpdateCartItemRequest request)
        {
            var cart = await _cartRepo.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return false;
            }

            var cartItem = await _cartRepo.GetCartItemByIdAsync(cartItemId);
            if (cartItem == null || cartItem.CartId != cart.CartId)
            {
                return false;
            }

            var product = await _productRepo.GetProductByIdAsync(cartItem.ProductId);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            if (product.Stock < request.Quantity)
            {
                throw new Exception($"Insufficient stock. Only {product.Stock} items available");
            }

            cartItem.Quantity = request.Quantity;
            await _cartRepo.UpdateCartItemAsync(cartItem);

            return true;
        }

        public async Task<bool> RemoveCartItemAsync(int userId, int cartItemId)
        {
            var cart = await _cartRepo.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return false;
            }

            var cartItem = await _cartRepo.GetCartItemByIdAsync(cartItemId);
            if (cartItem == null || cartItem.CartId != cart.CartId)
            {
                return false;
            }

            await _cartRepo.DeleteCartItemAsync(cartItem);
            return true;
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            var cart = await _cartRepo.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return false;
            }

            await _cartRepo.ClearCartAsync(cart.CartId);
            return true;
        }
    }
}
