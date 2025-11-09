using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.Services.DTOs.Requests;
using PetShop.Services.Interfaces;
using PetShop.Services.Services;
using System.Security.Claims;

namespace PetShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IEmailService _emailService;
        public OrdersController(IOrderService orderService, IEmailService emailService)
        {
            _orderService = orderService;
            _emailService = emailService;
        }

        [HttpPost("create-from-cart")]
        public async Task<IActionResult> CreateOrderFromCart([FromBody] CreateOrderRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            try
            {
                var order = await _orderService.CreateOrderFromCartAsync(userId, request);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMyOrders([FromQuery] string? status)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var orders = await _orderService.GetOrdersByUserIdAsync(userId, status);
            return Ok(orders);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllOrders([FromQuery] string? status)
        {
            var orders = await _orderService.GetAllOrdersAsync(status);
            return Ok(orders);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var order = await _orderService.GetOrderByIdAsync(orderId, userId);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            return Ok(order);
        }

        [HttpPut("{orderId}/cancel")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            try
            {
                var result = await _orderService.CancelOrderAsync(orderId, userId);
                if (!result)
                {
                    return NotFound(new { message = "Order not found" });
                }

                return Ok(new { message = "Order cancelled successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("status")]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusRequest request)
        {
            if (request == null || request.OrderId <= 0 || string.IsNullOrEmpty(request.Status))
            {
                return BadRequest(new { message = "Invalid request data" });
            }

            try
            {
                var result = await _orderService.UpdateOrderStatusAsync(request.OrderId, request.Status);
                if (!result)
                    return NotFound(new { message = "Order not found" });

                var order = await _orderService.GetOrderByIdAsync(request.OrderId);

                if (order != null)
                {
                    var subject = $"Cập nhật đơn hàng #{order.OrderId}";
                    var body = $"Xin chào {order.UserName},\n\n" +
                               $"Đơn hàng của bạn hiện đã được cập nhật sang trạng thái: {order.Status}.\n\n" +
                               $"Cảm ơn bạn đã mua sắm tại PetShop! 🐾";

                    await _emailService.SendOtpAsync(order.UserEmail, subject, body);
                }

                return Ok(new { message = "Order status updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
