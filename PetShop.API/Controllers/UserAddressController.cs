using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetShop.Services.DTOs.Requests;
using PetShop.Services.Interfaces;
using PetShop.Services.Services;

namespace PetShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserAddressController : ControllerBase
    {
        private readonly IUserAddressService _userAddressService;
        private readonly IMapboxService _mapboxService;

        public UserAddressController(IUserAddressService userAddressService, IMapboxService mapboxService)
        {
            _userAddressService = userAddressService;
            _mapboxService = mapboxService;
        }

        /// <summary>
        /// Tạo mới một địa chỉ cho người dùng.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] UserAddressRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.AddressLine))
                return BadRequest(new { message = "Địa chỉ không hợp lệ hoặc thiếu thông tin." });

            //string fullAddress = $"{request.AddressLine}, {request.Ward}, {request.District}, {request.City}";
            //var coordinates = await _mapboxService.GetCoordinatesFromAddress(fullAddress);

            //if (coordinates == null || coordinates.lat == null || coordinates.lon == null)
            //    return BadRequest(new { message = "Không tìm thấy vị trí phù hợp cho địa chỉ này." });

            //request.Latitude = coordinates.lat;
            //request.Longitude = coordinates.lon;

            var result = await _userAddressService.CreateAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Lấy địa chỉ mặc định của người dùng theo UserId.
        /// </summary>
        [HttpGet("default/{userId}")]
        public async Task<IActionResult> GetDefaultByUserId(int userId)
        {
            var result = await _userAddressService.GetDefaultByUserId(userId);
            if (result == null)
                return NotFound(new { message = "Không tìm thấy địa chỉ mặc định cho người dùng này." });

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserAddresses()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                           ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var addresses = await _userAddressService.GetByUserIdAsync(userId);
            return Ok(addresses);
        }

        /// <summary>
        /// Cập nhật địa chỉ người dùng theo ID.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UserAddressRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.AddressLine))
                return BadRequest(new { message = "Địa chỉ không hợp lệ hoặc thiếu thông tin." });

            //string fullAddress = $"{request.AddressLine}, {request.Ward}, {request.District}, {request.City}";
            //var coordinates = await _mapboxService.GetCoordinatesFromAddress(fullAddress);

            //if (coordinates == null || coordinates.lat == null || coordinates.lon == null)
            //    return BadRequest(new { message = "Không tìm thấy vị trí phù hợp cho địa chỉ này." });

            //request.Latitude = coordinates.lat;
            //request.Longitude = coordinates.lon;

            try
            {
                var result = await _userAddressService.UpdateAsync(id, request);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Không tìm thấy địa chỉ cần cập nhật." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật địa chỉ.", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy địa chỉ Admin.
        /// </summary>
        [HttpGet("admin")]
        public async Task<IActionResult> GetAdmin()
        {
            var result = await _userAddressService.GetAdmin();
            if (result == null)
                return NotFound(new { message = "Không tìm thấy địa chỉ admin." });

            return Ok(result);
        }

        [HttpPost("route")]
        public async Task<IActionResult> GetRoute([FromBody] RouteRequest req)
        {
            if (req == null ||
                req.StartLat == 0 || req.StartLng == 0 ||
                req.EndLat == 0 || req.EndLng == 0)
            {
                return BadRequest(new { message = "Tọa độ không hợp lệ." });
            }

            try
            {
                var route = await _mapboxService.GetRoute(req);

                if (route?.routes == null || !route.routes.Any())
                    return NotFound(new { message = "Không tìm thấy route." });

                var firstRoute = route.routes.First();

                // Code cũ: (Chỉ trả về coords)
                // var coords = route.routes.First().geometry.coordinates;
                // return Ok(new { coordinates = coords });

                // Code mới: (Trả về cả 3)
                return Ok(new
                {
                    coordinates = firstRoute.geometry.coordinates,
                    distance = firstRoute.distance,
                    duration = firstRoute.duration
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi lấy route", detail = ex.Message });
            }
        }
    }

}
