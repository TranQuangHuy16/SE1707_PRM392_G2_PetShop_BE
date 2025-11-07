namespace PetShop.Services.Interfaces
{
    public interface IProductRatingService
    {
        Task<bool> AddRatingAsync(int userId, int productId, int stars, string comment);
        Task<object> GetRatingsByProductIdAsync(int productId);
    }
}
