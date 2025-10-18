using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using PetShop.Services.DTOs.Requests;
using PetShop.Services.Interfaces;
using LoginRequest = PetShop.Services.DTOs.Requests.LoginRequest;
using RegisterRequest = PetShop.Services.DTOs.Requests.RegisterRequest;

namespace PetShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IOtpService _otpService;

        public AuthController(IAuthService authService, IOtpService otpService)
        {
            _authService = authService;
            _otpService = otpService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var result = await _authService.Register(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var token = await _authService.Login(request);
                return Ok(new { accessToken = token });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("login-by-google")]
        public async Task<IActionResult> LoginByGoogle([FromBody] GoogleLoginRequest request)
        {
            try
            {
                var token = await _authService.LoginWithGoogleAsync(request);
                return Ok(new { accessToken = token });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("login-by-facebook")]
        public async Task<IActionResult> LoginByFacebook([FromBody] LoginFacebookRequest request)
        {
            try
            {
                var token = await _authService.LoginWithFacebookAsync(request);
                return Ok(new { accessToken = token });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            await _authService.LogoutAsync(token);
            return NoContent();
        }

        // 1. Request OTP
        [HttpPost("forgot-password/request")]
        public async Task<IActionResult> RequestOtp([FromBody] RequestOtpDto dto)
        {
            // return 200 even if email doesn't exist to avoid leaking user list
            var ok = await _otpService.RequestOtpAsync(dto.Email);
            return Ok(true);
        }

        // 2. Verify OTP
        [HttpPost("forgot-password/verify")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            var valid = await _otpService.VerifyOtpAsync(dto.Email, dto.Code);
            if (!valid) return BadRequest(new { success = false, message = "Invalid or expired OTP." });
            return Ok(true);
        }

        // 3. Reset password
        [HttpPost("forgot-password/reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var user = await _otpService.ResetPasswordAsync(dto.Email, dto.NewPassword);
            if (user == null)
                return BadRequest(new { success = false, message = "Không tìm thấy người dùng hoặc OTP không hợp lệ/hết hạn." });

            return Ok(user);
        }
    }
}
