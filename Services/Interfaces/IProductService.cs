using Inventory_Mgmt_System.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory_Mgmt_System.Services.Interfaces
{
    public interface IProductService
    {
        Task<Product> GetProductByIdAsync(Guid id);

        Task<Product> CreateProductAsync(Product product);

        Task<Product> UpdateProductAsync(Product product);

        Task<bool> DeleteProductAsync(Guid id);

        Task<List<Product>> GetProductsByCategoryAsync(Guid categoryId);

        Task<List<Product>> GetProductsByUserAsync(Guid userId);

        Task<List<Product>> GetAllProductsAsync();
    }
}
