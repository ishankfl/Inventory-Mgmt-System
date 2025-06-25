using Inventory_Mgmt_System.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory_Mgmt_System.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> GetProductById(Guid id);
        Task<Product> GetProductByName(string name);

        Task<Product> CreateProduct(Product product);

        Task<Product> UpdateProduct(Product updatedProduct);

        Task<bool> DeleteProduct(Guid id);

        Task<List<Product>> GetProductsByCategory(Guid categoryId);

        Task<List<Product>> GetProductsByUser(Guid userId);

        Task<List<Product>> GetAllProducts();

        Task<List<Product>> GetTopTenProductsByQty();

        Task<int> TotalNumberOfProduct();



    }
}
