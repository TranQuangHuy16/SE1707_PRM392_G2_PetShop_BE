using PetShop.Services.DTOs.Requests;
using PetShop.Services.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponse>> GetAllProductsAsync();
        Task<ProductResponse?> GetProductByIdAsync(int productId);
        Task<ProductResponse> CreateProductAsync(ProductCreateRequest request);
        Task<bool> UpdateProductAsync(int productId, ProductUpdateRequest request);
        Task<bool> DeleteProductAsync(int productId);
        Task<IEnumerable<ProductResponse>> GetProductsByCategoryIdAsync(int categoryId);
    }
}
