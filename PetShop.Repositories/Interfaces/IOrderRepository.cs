using PetShop.Repositories.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetShop.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> CreateOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(Order order);

        Task<OrderDetail> AddOrderDetailAsync(OrderDetail orderDetail);
        Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId);
    }
}
