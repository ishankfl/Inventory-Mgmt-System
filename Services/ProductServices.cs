using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Inventory_Mgmt_System.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory_Mgmt_System.Services
{
    public class ProductServices : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IActivityRepository _activityRepository;

        public ProductServices(IProductRepository productRepository, IUserRepository userRepository, IActivityRepository activityRepository)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
            _activityRepository = activityRepository;
        }

        public async Task<Product> CreateProductWithActivity(AddProductDto dto, Guid currentUserId)
        {
            var existingProduct = await _productRepository.GetProductByName(dto.Name);
            if (existingProduct != null)
                throw new InvalidOperationException($"Product with this name already exists: {dto.Name}");

            var newProduct = new Product
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Quantity = dto.Quantity,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                UserId = dto.UserId,
                CreatedAt = DateTime.UtcNow
            };

            var createdProduct = await _productRepository.CreateProduct(newProduct);

            var currentUser = await _userRepository.GetUserById(currentUserId);

            await _activityRepository.AddNewActivity(new Activity
            {
                Action = $"User with Email {currentUser.Email} added new product: {newProduct.Name}",
                Status = "success",
                Type = ActivityType.ProductAdded,
                UserId = currentUserId
            });

            return createdProduct;
        }

        public async Task<Product> UpdateProductWithActivity(Guid id, AddProductDto dto, Guid currentUserId)
        {
            var existingProduct = await _productRepository.GetProductById(id);
            if (existingProduct == null)
                throw new KeyNotFoundException($"Product with ID {id} not found.");

            var productWithSameName = await _productRepository.GetProductByName(dto.Name);
            if (productWithSameName != null && productWithSameName.Id != existingProduct.Id)
                throw new InvalidOperationException($"Product with name '{dto.Name}' already exists.");

            existingProduct.Name = dto.Name;
            existingProduct.Description = dto.Description;
            existingProduct.Quantity = dto.Quantity;
            existingProduct.Price = dto.Price;
            existingProduct.CategoryId = dto.CategoryId;
            existingProduct.UserId = dto.UserId;

            var updatedProduct = await _productRepository.UpdateProduct(existingProduct);

            var currentUser = await _userRepository.GetUserById(currentUserId);

            await _activityRepository.AddNewActivity(new Activity
            {
                Action = $"User with Email {currentUser.Email} updated the product: {updatedProduct.Name}",
                Status = "warning",
                Type = ActivityType.ProductUpdated,
                UserId = currentUserId
            });

            return updatedProduct;
        }

        public async Task DeleteProductWithActivity(Guid id, Guid currentUserId)
        {
            var existingProduct = await _productRepository.GetProductById(id);
            if (existingProduct == null)
                throw new KeyNotFoundException($"Product with ID {id} not found.");

            await _productRepository.DeleteProduct(id);

            var currentUser = await _userRepository.GetUserById(currentUserId);

            await _activityRepository.AddNewActivity(new Activity
            {
                Action = $"User with Email {currentUser.Email} deleted the product: {existingProduct.Name}",
                Status = "danger",
                Type = ActivityType.ProductDeleted,
                UserId = currentUserId
            });
        }

        public async Task<Product> CreateProductAsync(Product product) => await _productRepository.CreateProduct(product);

        public async Task<bool> DeleteProductAsync(Guid id) => await _productRepository.DeleteProduct(id);

        public async Task<List<Product>> GetAllProductsAsync() => await _productRepository.GetAllProducts();

        public async Task<Product> GetProductByIdAsync(Guid id) => await _productRepository.GetProductById(id);

        public async Task<Product> GetProductByNameAsync(string name) => await _productRepository.GetProductByName(name);

        public async Task<List<Product>> GetProductsByCategoryAsync(Guid categoryId) => await _productRepository.GetProductsByCategory(categoryId);

        public async Task<List<Product>> GetProductsByUserAsync(Guid userId) => await _productRepository.GetProductsByUser(userId);

        public async Task<Product> UpdateProductAsync(Product product) => await _productRepository.UpdateProduct(product);
    }
}
