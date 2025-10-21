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
    public class ProductRepository : IProductRepository
    {
        private readonly PetShopDbContext _context;

        public ProductRepository(PetShopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                                 .Include(p => p.Category)
                                 .Where(p => p.IsActive)
                                 .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _context.Products
                                 .Include(p => p.Category)
                                 .FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            var categoryStub = new Category { CategoryId = product.CategoryId };
            // Attach nó vào DbContext, báo là nó ĐÃ TỒN TẠI (Unchanged)
            _context.Attach(categoryStub);
            // Gán categoryStub vào navigation property (tùy chọn nhưng nên làm)
            product.Category = categoryStub;
            // ======================

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync(); // <-- Đặt lại breakpoint ở đây để kiểm tra
            return product;
        }

        public async Task UpdateProductAsync(Product product)
        {
            //_context.Products.Update(product); 
            _context.Entry(product).State = EntityState.Modified; 
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CategoryExistsAsync(int categoryId)
        {
            return await _context.Categories.AnyAsync(c => c.CategoryId == categoryId);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int categoryId)
        {
            return await _context.Products
                                 .Include(p => p.Category)
                                 .Where(p => p.CategoryId == categoryId && p.IsActive)
                                 .ToListAsync();
        }
    }
}
