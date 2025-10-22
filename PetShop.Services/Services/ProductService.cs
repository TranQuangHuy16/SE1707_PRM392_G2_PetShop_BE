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
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepo, IMapper mapper)
        {
            _productRepo = productRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductResponse>> GetAllProductsAsync()
        {
            var products = await _productRepo.GetAllProductsAsync();
            return _mapper.Map<IEnumerable<ProductResponse>>(products);
        }

        public async Task<ProductResponse?> GetProductByIdAsync(int productId)
        {
            var product = await _productRepo.GetProductByIdAsync(productId);
            if (product == null)
            {
                return null;
            }
            return _mapper.Map<ProductResponse>(product);
        }

        public async Task<ProductResponse> CreateProductAsync(ProductCreateRequest request)
        {
            if (!await _productRepo.CategoryExistsAsync(request.CategoryId))
            {
                throw new System.Exception("Category does not exist.");
            }

            var product = _mapper.Map<Product>(request);

            var newProduct = await _productRepo.AddProductAsync(product);
            var productForResponse = await _productRepo.GetProductByIdAsync(newProduct.ProductId);

            return _mapper.Map<ProductResponse>(productForResponse);
        }

        public async Task<bool> UpdateProductAsync(int productId, ProductUpdateRequest request)
        {
            var existingProduct = await _productRepo.GetProductByIdAsync(productId);
            if (existingProduct == null)
            {
                return false;
            }

            if (request.CategoryId != existingProduct.CategoryId)
            {
                if (!await _productRepo.CategoryExistsAsync(request.CategoryId))
                {
                    throw new System.Exception("New Category does not exist.");
                }
            }
            _mapper.Map(request, existingProduct);

            await _productRepo.UpdateProductAsync(existingProduct);
            return true;
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await _productRepo.GetProductByIdAsync(productId);
            if (product == null)
            {
                return false;
            }

            product.IsActive = false;
            await _productRepo.UpdateProductAsync(product);
            return true;
        }

        public async Task<IEnumerable<ProductResponse>> GetProductsByCategoryIdAsync(int categoryId)
        {
            var products = await _productRepo.GetProductsByCategoryIdAsync(categoryId);
            return _mapper.Map<IEnumerable<ProductResponse>>(products);
        }
    }
}
