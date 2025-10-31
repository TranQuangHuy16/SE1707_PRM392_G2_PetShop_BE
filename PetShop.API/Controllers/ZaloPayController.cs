using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.Services.Interfaces;
using System.Security.Claims;
using static PetShop.Services.Interfaces.IZaloPayService;

namespace PetShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZaloPayController : ControllerBase
    {
        private readonly IZaloPayService _zaloPayService;

        public ZaloPayController(IZaloPayService zaloPayService)
        {
            _zaloPayService = zaloPayService;
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreatePayment([FromBody] CreateZaloPayRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            try
            {
                var result = await _zaloPayService.CreatePaymentAsync(request.OrderId, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("callback")]
        [AllowAnonymous]
        public async Task<IActionResult> Callback([FromBody] ZaloPayCallbackData request)
        {
            try
            {
                var result = await _zaloPayService.ProcessCallbackAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("query/{orderId}")]
        [Authorize]
        public async Task<IActionResult> QueryOrderStatus(int orderId)
        {
            try
            {
                var result = await _zaloPayService.QueryOrderStatusAsync(orderId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class CreateZaloPayRequest
    {
        public int OrderId { get; set; }
    }


}
