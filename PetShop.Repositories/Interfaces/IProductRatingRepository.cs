using PetShop.Repositories.Models;

namespace PetShop.Repositories.Interfaces
{
    public interface IProductRatingRepository
    {
        Task AddRatingAsync(ProductRating rating);
        Task<IEnumerable<ProductRating>> GetRatingsByProductIdAsync(int productId);
        Task<bool> HasUserRatedProductAsync(int userId, int productId);
    }
}
