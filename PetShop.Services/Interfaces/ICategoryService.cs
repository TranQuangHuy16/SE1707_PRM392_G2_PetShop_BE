using PetShop.Services.DTOs.Requests;
using PetShop.Services.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync();
        Task<CategoryResponse?> GetCategoryByIdAsync(int categoryId);
        Task<CategoryResponse> CreateCategoryAsync(CategoryCreateRequest request);
        Task<bool> UpdateCategoryAsync(int categoryId, CategoryUpdateRequest request);
        Task<bool> DeleteCategoryAsync(int categoryId);
    }
}
