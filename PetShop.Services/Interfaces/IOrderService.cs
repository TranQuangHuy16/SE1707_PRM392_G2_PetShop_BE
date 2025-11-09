using PetShop.Services.DTOs.Requests;
using PetShop.Services.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetShop.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateOrderFromCartAsync(int userId, CreateOrderRequest request);
        Task<OrderResponse?> GetOrderByIdAsync(int orderId, int userId);
        Task<OrderResponse?> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<OrderResponse>> GetOrdersByUserIdAsync(int userId, string? status = null);
        Task<IEnumerable<OrderResponse>> GetAllOrdersAsync(string? status = null);
        Task<bool> CancelOrderAsync(int orderId, int userId);
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);

    }
}
