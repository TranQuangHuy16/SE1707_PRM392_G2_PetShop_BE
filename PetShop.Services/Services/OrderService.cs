using PetShop.Repositories.Interfaces;
using PetShop.Repositories.Models;
using PetShop.Repositories.Models.Enums;
using PetShop.Services.DTOs.Requests;
using PetShop.Services.DTOs.Responses;
using PetShop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetShop.Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ICartRepository _cartRepo;
        private readonly IProductRepository _productRepo;
        private readonly IUserAddressRepository _addressRepo;

        public OrderService(
            IOrderRepository orderRepo, 
            ICartRepository cartRepo, 
            IProductRepository productRepo,
            IUserAddressRepository addressRepo)
        {
            _orderRepo = orderRepo;
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            _addressRepo = addressRepo;
        }

        public async Task<OrderResponse> CreateOrderFromCartAsync(int userId, CreateOrderRequest request)
        {
            // Get user's cart
            var cart = await _cartRepo.GetCartByUserIdAsync(userId);
            if (cart == null || !cart.CartItems.Any())
            {
                throw new Exception("Cart is empty");
            }

            // Validate address if provided
            if (request.AddressId.HasValue)
            {
                var address = await _addressRepo.GetByIdAsync(request.AddressId.Value);
                if (address == null || address.UserId != userId)
                {
                    throw new Exception("Invalid address");
                }
            }

            // Calculate total and validate stock
            decimal totalAmount = 0;
            foreach (var cartItem in cart.CartItems)
            {
                var product = await _productRepo.GetProductByIdAsync(cartItem.ProductId);
                if (product == null || !product.IsActive)
                {
                    throw new Exception($"Product '{cartItem.Product.ProductName}' is no longer available");
                }

                if (product.Stock < cartItem.Quantity)
                {
                    throw new Exception($"Insufficient stock for product '{product.ProductName}'. Available: {product.Stock}");
                }

                totalAmount += product.Price * cartItem.Quantity;
            }

            // Create order
            var order = new Order
            {
                UserId = userId,
                AddressId = request.AddressId,
                OrderDate = DateTime.Now,
                TotalAmount = totalAmount,
                Status = OrderStatusEnum.Pending,
                IsActive = true
            };

            order = await _orderRepo.CreateOrderAsync(order);

            // Create order details and update product stock
            foreach (var cartItem in cart.CartItems)
            {
                var product = await _productRepo.GetProductByIdAsync(cartItem.ProductId);
                
                var orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = product!.Price
                };

                await _orderRepo.AddOrderDetailAsync(orderDetail);

                product.Stock -= cartItem.Quantity;
                await _productRepo.UpdateProductAsync(product);
            }

            await _cartRepo.ClearCartAsync(cart.CartId);

            return await GetOrderByIdAsync(order.OrderId, userId) 
                ?? throw new Exception("Order created but could not be retrieved");
        }

        public async Task<OrderResponse?> GetOrderByIdAsync(int orderId, int userId)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId);
            if (order == null || order.UserId != userId)
            {
                return null;
            }

            return MapToOrderResponse(order);
        }

        public async Task<IEnumerable<OrderResponse>> GetOrdersByUserIdAsync(int userId, string? status = null)
        {
            var orders = await _orderRepo.GetOrdersByUserIdAsync(userId);
            
            // Filter by status if provided
            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse<OrderStatusEnum>(status, true, out var statusEnum))
                {
                    orders = orders.Where(o => o.Status == statusEnum);
                }
            }
            
            return orders.Select(MapToOrderResponse).ToList();
        }

        public async Task<IEnumerable<OrderResponse>> GetAllOrdersAsync(string? status = null)
        {
            var orders = await _orderRepo.GetAllOrdersAsync();
            
            // Filter by status if provided
            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse<OrderStatusEnum>(status, true, out var statusEnum))
                {
                    orders = orders.Where(o => o.Status == statusEnum);
                }
            }
            
            return orders.Select(MapToOrderResponse).ToList();
        }

        public async Task<bool> CancelOrderAsync(int orderId, int userId)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId);
            if (order == null || order.UserId != userId)
            {
                return false;
            }

            if (order.Status != OrderStatusEnum.Pending)
            {
                throw new Exception("Only pending orders can be cancelled");
            }

            // Restore product stock
            foreach (var detail in order.OrderDetails)
            {
                var product = await _productRepo.GetProductByIdAsync(detail.ProductId);
                if (product != null)
                {
                    product.Stock += detail.Quantity;
                    await _productRepo.UpdateProductAsync(product);
                }
            }

            order.Status = OrderStatusEnum.Cancelled;
            await _orderRepo.UpdateOrderAsync(order);

            return true;
        }

        private OrderResponse MapToOrderResponse(Order order)
        {
            return new OrderResponse
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                AddressId = order.AddressId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                Address = order.Address != null ? new UserAddressResponse
                {
                    AddressId = order.Address.AddressId,
                    UserId = order.Address.UserId,
                    AddressLine = order.Address.AddressLine,
                    Ward = order.Address.Ward,
                    District = order.Address.District,
                    City = order.Address.City,
                    Province = order.Address.City,
                    PostalCode = order.Address.PostalCode,
                    IsDefault = order.Address.IsDefault,
                    Latitude = order.Address.Latitude,
                    Longitude = order.Address.Longitude
                } : null,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailResponse
                {
                    OrderDetailId = od.OrderDetailId,
                    ProductId = od.ProductId,
                    ProductName = od.Product.ProductName,
                    ProductImageUrl = od.Product.ImageUrl ?? string.Empty,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    TotalPrice = od.UnitPrice * od.Quantity
                }).ToList()
            };
        }
    }
}
