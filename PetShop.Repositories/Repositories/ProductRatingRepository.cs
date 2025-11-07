using Microsoft.EntityFrameworkCore;
using PetShop.Repositories.DBContext;
using PetShop.Repositories.Interfaces;
using PetShop.Repositories.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetShop.Repositories.Repositories
{
    public class ProductRatingRepository : IProductRatingRepository
    {
        private readonly PetShopDbContext _context;

        public ProductRatingRepository(PetShopDbContext context)
        {
            _context = context;
        }

        public async Task AddRatingAsync(ProductRating rating)
        {
            _context.ProductRatings.Add(rating);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductRating>> GetRatingsByProductIdAsync(int productId)
        {
            return await _context.ProductRatings
                .Where(r => r.ProductId == productId)
                .Include(r => r.User)
                .ToListAsync();
        }

        public async Task<bool> HasUserRatedProductAsync(int userId, int productId)
        {
            return await _context.ProductRatings
                .AnyAsync(r => r.UserId == userId && r.ProductId == productId);
        }
    }
}
