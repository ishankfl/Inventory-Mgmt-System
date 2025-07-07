using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory_Mgmt_System.Services.Interfaces
{
    public interface IProductService
    {
        // Basic CRUD
        Task<Product> GetProductByIdAsync(Guid id);
        Task<Product> CreateProductAsync(Product product);
        Task<Product> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(Guid id);

        // Queries
        Task<List<Product>> GetProductsByCategoryAsync(Guid categoryId);
        Task<List<Product>> GetProductsByUserAsync(Guid userId);
        Task<List<Product>> GetAllProductsAsync();
        Task<Product> GetProductByNameAsync(string name);

        // Extended business logic with activity tracking
        Task<Product> CreateProductWithActivity(AddProductDto dto, Guid currentUserId);
        Task<Product> UpdateProductWithActivity(Guid id, AddProductDto dto, Guid currentUserId);
        Task DeleteProductWithActivity(Guid id, Guid currentUserId);
    }
}
