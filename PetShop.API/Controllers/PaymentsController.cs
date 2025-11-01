using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.Services.DTOs.Requests;
using PetShop.Services.Interfaces;
using System.Security.Claims;

namespace PetShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPayments([FromQuery] int? status = null)
        {
            PetShop.Repositories.Models.Enums.PaymentStatusEnum? paymentStatus = null;
            if (status.HasValue)
            {
                paymentStatus = (PetShop.Repositories.Models.Enums.PaymentStatusEnum)status.Value;
            }

            var payments = await _paymentService.GetAllPaymentsAsync(paymentStatus);
            return Ok(payments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return NotFound(new { message = "Payment not found" });
            }
            return Ok(payment);
        }

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetPaymentByOrderId(int orderId)
        {
            var payment = await _paymentService.GetPaymentByOrderIdAsync(orderId);
            if (payment == null)
            {
                return NotFound(new { message = "Payment not found for this order" });
            }
            return Ok(payment);
        }

        [HttpGet("my-payments")]
        public async Task<IActionResult> GetMyPayments([FromQuery] int? status = null)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            PetShop.Repositories.Models.Enums.PaymentStatusEnum? paymentStatus = null;
            if (status.HasValue)
            {
                paymentStatus = (PetShop.Repositories.Models.Enums.PaymentStatusEnum)status.Value;
            }

            var payments = await _paymentService.GetPaymentsByUserIdAsync(userId, paymentStatus);
            return Ok(payments);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            try
            {
                var newPayment = await _paymentService.CreatePaymentAsync(request);
                return CreatedAtAction(nameof(GetPaymentById), new { id = newPayment.PaymentId }, newPayment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] UpdatePaymentStatusRequest request)
        {
            var result = await _paymentService.UpdatePaymentStatusAsync(id, request);
            if (!result)
            {
                return NotFound(new { message = "Payment not found" });
            }
            return Ok(new { message = "Payment status updated successfully" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var result = await _paymentService.DeletePaymentAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Payment not found" });
            }
            return Ok(new { message = "Payment deleted successfully" });
        }

        [HttpPost("zalopay-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> ZaloPayCallback([FromBody] Dictionary<string, object> cbdata)
        {
            try
            {
                var result = await _paymentService.ProcessZaloPayCallbackAsync(cbdata);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new { return_code = 0, return_message = ex.Message });
            }
        }

        [HttpPost("confirm/{orderId}")]
        [Authorize]
        public async Task<IActionResult> ConfirmPayment(int orderId)
        {
            try
            {
                var result = await _paymentService.ConfirmPaymentByOrderIdAsync(orderId);
                if (result)
                {
                    return Ok(new { success = true, message = "Payment confirmed successfully" });
                }
                return NotFound(new { success = false, message = "Payment not found or already confirmed" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
