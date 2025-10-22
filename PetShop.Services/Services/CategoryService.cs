using AutoMapper;
using PetShop.Repositories.Interfaces;
using PetShop.Repositories.Models;
using PetShop.Services.DTOs.Requests;
using PetShop.Services.DTOs.Responses;
using PetShop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Services.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepo, IMapper mapper)
        {
            _categoryRepo = categoryRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepo.GetAllCategoriesAsync();
            return _mapper.Map<IEnumerable<CategoryResponse>>(categories);
        }

        public async Task<CategoryResponse?> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _categoryRepo.GetCategoryByIdAsync(categoryId);
            if (category == null)
            {
                return null;
            }
            return _mapper.Map<CategoryResponse>(category);
        }

        public async Task<CategoryResponse> CreateCategoryAsync(CategoryCreateRequest request)
        {
            var category = _mapper.Map<Category>(request);
            var newCategory = await _categoryRepo.AddCategoryAsync(category);

            return _mapper.Map<CategoryResponse>(newCategory);
        }

        public async Task<bool> UpdateCategoryAsync(int categoryId, CategoryUpdateRequest request)
        {
            var existingCategory = await _categoryRepo.GetCategoryByIdAsync(categoryId);
            if (existingCategory == null)
            {
                return false;
            }

            _mapper.Map(request, existingCategory);
            await _categoryRepo.UpdateCategoryAsync(existingCategory);
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            if (await _categoryRepo.CategoryHasProductsAsync(categoryId))
            {
                throw new System.Exception("Cannot delete category: Products are associated with it.");
            }

            var category = await _categoryRepo.GetCategoryByIdAsync(categoryId);
            if (category == null)
            {
                return false;
            }

            await _categoryRepo.DeleteCategoryAsync(category);
            return true;
        }
    }
}
