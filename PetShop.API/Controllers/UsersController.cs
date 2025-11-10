using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.API.DTOs;
using PetShop.Services;
using PetShop.Services.DTOs.Requests;
using PetShop.Services.Interfaces;
using System.Security.Claims;

namespace PetShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAll();
            return Ok(users);
        }

        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                throw new Exception("Invalid token");

            var user = await _userService.GetById(userId);

            return Ok(user);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {

            var user = await _userService.GetById(id);

            return Ok(user);
        }

        [HttpGet("detail/{id}")]
        [Authorize(Roles = "Admin, Customer")]
        public async Task<IActionResult> GetUserDetailAsync(int id)
        {
            var user = await _userService.GetDetailAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                UserDetailResponseDto updatedUser = await _userService.UpdateUserAsync(id, request);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                int res = await _userService.DeleteUserAsync(id);
                if (res == 0)
                {
                    return NotFound(new { message = "User not found or could not be deleted." });
                }
                return Ok(new { message = "User deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/fcm-token")]
        public async Task<IActionResult> UpdateFcmToken(int id, [FromBody] string fcmToken)
        {
            var success = await _userService.UpdateFcmTokenAsync(id, fcmToken);
            if (!success) return NotFound(new { message = "User not found" });

            return Ok(new { success = true });
        }

        [HttpPut("detail/{id}")]
        public async Task<IActionResult> UpdateUserDetails(int id, [FromBody] UpdateUserDetailsRequest request)
        {
            try
            {
                var updatedUser = await _userService.UpdateUserDetailsAsync(id, request);

                if (updatedUser == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
