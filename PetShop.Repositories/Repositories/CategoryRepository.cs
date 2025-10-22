using Microsoft.EntityFrameworkCore;
using PetShop.Repositories.DBContext;
using PetShop.Repositories.Interfaces;
using PetShop.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Repositories.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly PetShopDbContext _context;

        public CategoryRepository(PetShopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            return await _context.Categories
                                 .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        }

        public async Task<Category> AddCategoryAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CategoryHasProductsAsync(int categoryId)
        {
            return await _context.Products.AnyAsync(p => p.CategoryId == categoryId);
        }
    }
}
