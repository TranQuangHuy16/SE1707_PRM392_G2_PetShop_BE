using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.Services.Interfaces;

namespace PetShop.API.Controllers
{
    [Route("api/products/{productId}/ratings")]
    [ApiController]
    [Authorize]
    public class ProductRatingsController : ControllerBase
    {
        private readonly IProductRatingService _ratingService;
        public ProductRatingsController(IProductRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        // POST: api/products/5/ratings
        [HttpPost]
        public async Task<IActionResult> AddRating(int productId, [FromBody] RatingRequest request)
        {
            await _ratingService.AddRatingAsync(request.UserId, productId, request.Stars, request.Comment);
            return Ok(new { Message = "Rating added successfully." });
        }

        // GET: api/products/5/ratings
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetRatings(int productId)
        {
            var result = await _ratingService.GetRatingsByProductIdAsync(productId);
            return Ok(result);
        }
    }

    public class RatingRequest
    {
        public int UserId { get; set; }
        public int Stars { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
