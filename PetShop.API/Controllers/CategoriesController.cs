using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.Services.DTOs.Requests;
using PetShop.Services.Interfaces;

namespace PetShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

      
        // GET: api/categories
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        
        // GET: api/categories/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        // POST: api/categories
        [HttpPost]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateRequest request)
        {
            var newCategory = await _categoryService.CreateCategoryAsync(request);
            return CreatedAtAction(nameof(GetCategoryById), new { id = newCategory.CategoryId }, newCategory);
        }

      
        // PUT: api/categories/5
        [HttpPut("{id}")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateRequest request)
        {
            var result = await _categoryService.UpdateCategoryAsync(id, request);
            if (!result)
            {
                return NotFound();
            }
            return Ok(result);
        }

     
        // DELETE: api/categories/5
        [HttpDelete("{id}")]
        // [Authorize(Roles ="Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
