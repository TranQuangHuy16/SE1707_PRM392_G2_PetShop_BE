using PetShop.Repositories.Interfaces;
using PetShop.Repositories.Models;
using PetShop.Services.Interfaces;

namespace PetShop.Services.Services
{
    public class ProductRatingService : IProductRatingService
    {
        private readonly IProductRatingRepository _repo;

        public ProductRatingService(IProductRatingRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> AddRatingAsync(int userId, int productId, int stars, string comment)
        {
            if (await _repo.HasUserRatedProductAsync(userId, productId))
                throw new Exception("User has already rated this product.");

            var rating = new ProductRating
            {
                UserId = userId,
                ProductId = productId,
                Stars = stars,
                Comment = comment,
                CreatedAt = DateTime.Now
            };

            await _repo.AddRatingAsync(rating);
            return true;
        }

        public async Task<object> GetRatingsByProductIdAsync(int productId)
        {
            var ratings = await _repo.GetRatingsByProductIdAsync(productId);
            var average = ratings.Any() ? ratings.Average(r => r.Stars) : 0;

            return new
            {
                AverageStars = Math.Round(average, 1),
                Count = ratings.Count(),
                Ratings = ratings.Select(r => new
                {
                    r.Id,
                    r.UserId,
                    r.Stars,
                    r.Comment,
                    UserName = r.User.FullName,
                    r.CreatedAt
                })
            };
        }
    }
}
