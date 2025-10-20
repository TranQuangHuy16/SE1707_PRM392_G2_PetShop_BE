using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetShop.Services.DTOs.Requests;
using PetShop.Services.Interfaces;

namespace PetShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase 
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        
        // GET: api/products
        [HttpGet]
        [AllowAnonymous] 
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

      
        // GET: api/products/{id}
        [HttpGet("{id}")]
        [AllowAnonymous] 
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound(); 
            }

            return Ok(product);
        }

      
        // POST: api/products
        [HttpPost]
        // [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateRequest request)
        {

            try
            {
                var newProduct = await _productService.CreateProductAsync(request);

                return CreatedAtAction(nameof(GetProductById), new { id = newProduct.ProductId }, newProduct);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        // [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateRequest request)
        {
            try
            {
                var result = await _productService.UpdateProductAsync(id, request);
                if (!result)
                {
                    return NotFound(); 
                }

                return Ok(result); 
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        // [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result)
            {
                return NotFound(); 
            }

            return Ok(result);
        }

        // GET: api/products/category/5
        [HttpGet("category/{categoryId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductsByCategoryId(int categoryId)
        {
            var products = await _productService.GetProductsByCategoryIdAsync(categoryId);
            return Ok(products);
        }
    }
}
