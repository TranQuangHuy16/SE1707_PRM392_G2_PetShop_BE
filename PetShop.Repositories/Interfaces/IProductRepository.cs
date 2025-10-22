using PetShop.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int productId);
        Task<Product> AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(Product product);
        Task<bool> CategoryExistsAsync(int categoryId);

        Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int categoryId);
    }
}
