using Inventory_Mgmt_System.Data;
using Inventory_Mgmt_System.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Dapper;

namespace Inventory_Mgmt_System.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        private readonly DapperDbContext _dapperDbContext;

        public ProductRepository(AppDbContext context, DapperDbContext dapperDbContext)
        {
            _context = context;
            _dapperDbContext = dapperDbContext;
        }

        public async Task<Product> CreateProduct(Product product)
        {
            var added = await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return added.Entity;
        }
        public async Task<Product> GetProductById(Guid id)
        {
            using (var dbConnection = _dapperDbContext.CreateConnection())
            {
                string query = @"
                    SELECT 
                        p.*, 
                        c.*, 
                        u.*
                    FROM ""Products"" p
                    JOIN ""Categories"" c ON p.""CategoryId"" = c.""Id""
                    JOIN ""Users"" u ON p.""UserId"" = u.""Id""
                    WHERE p.""Id"" = @Id";

                var product = (await dbConnection.QueryAsync<Product, Category, User, Product>(
                    query,
                    (p, category, user) =>
                    {
                        p.Category = category;
                        p.User = user;
                        return p;
                    },
                    new { Id = id },
                    splitOn: "Id,Id,Id" 
                )).FirstOrDefault();

                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {id} not found.");
                }

                return product;
            }
        }

        public async Task<Product> GetProductByName(string name)
        {
            using (var dbConnection = _dapperDbContext.CreateConnection())
            {
                string query = @"
        SELECT 
            ""Id"", ""Name"", ""Description"", ""Quantity"", ""Price"",
            ""CategoryId"", ""UserId"", ""CreatedAt""
        FROM ""Products""
        WHERE ""Name"" = @Name
        LIMIT 1;";

                var product = await dbConnection.QueryFirstOrDefaultAsync<Product>(
                    query,
                    new { Name = name }
                );


                return product;
            }
        }

        public async Task<List<Product>> GetAllProducts()
        {
            using (var dbConnection = _dapperDbContext.CreateConnection())
            {
                string query = @"
        SELECT 
            p.*, c.*, u.*
        FROM ""Products"" p
        JOIN ""Categories"" c ON p.""CategoryId"" = c.""Id""
        JOIN ""Users"" u ON p.""UserId"" = u.""Id""
        ORDER BY p.""CreatedAt"" DESC";

             
                var products = (await dbConnection.QueryAsync<Product, Category, User, Product>(
                    query,
                    (product, category, user) =>
                    {
                        product.Category = category;
                        product.User = user;
                        return product;
                    },
                    splitOn: "Id" 
                )).ToList();

                return products;
            }
        }



        public async Task<List<Product>> GetProductsByCategory(Guid categoryId)
        {
            using (var dbConnection = _dapperDbContext.CreateConnection())
            {
                string query = @"
                    SELECT 
                        p.*, c.*, u.*
                    FROM ""Products"" p
                    JOIN ""Categories"" c ON p.""CategoryId"" = c.""Id""
                    JOIN ""Users"" u ON p.""UserId"" = u.""Id""
                    WHERE p.""CategoryId"" = @CategoryId
                    ORDER BY p.""CreatedAt"" DESC";

                var products = (await dbConnection.QueryAsync<Product, Category, User, Product>(
                    query,
                    (product, category, user) =>
                    {
                        product.Category = category;
                        product.User = user;
                        return product;
                    },
                    new { CategoryId = categoryId },
                    splitOn: "Id,Id,Id"
                )).AsList();

                return products;
            }
        }

        public async Task<List<Product>> GetProductsByUser(Guid userId)
        {
            var products = await _context.Products
                .Where(p => p.UserId == userId)
                .Include(p => p.Category)
                .Include(p => p.User)
                .ToListAsync();

            return products ?? new List<Product>();
        }

        public async Task<Product> UpdateProduct(Product updatedProduct)
        {
            var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == updatedProduct.Id);

            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Product with ID {updatedProduct.Id} not found.");
            }

            existingProduct.Name = updatedProduct.Name;
            existingProduct.CategoryId = updatedProduct.CategoryId;
            existingProduct.UserId = updatedProduct.UserId;
            existingProduct.Price = updatedProduct.Price;
            existingProduct.Quantity = updatedProduct.Quantity;
            existingProduct.Description = updatedProduct.Description; ;



            await _context.SaveChangesAsync();

            return existingProduct;
        }

        public async Task<bool> DeleteProduct(Guid id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return false;
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<Product>> GetTopTenProductsByQty()
        {
            var top10Products = await _context.Products
                .OrderByDescending(p => p.Quantity)
                .Take(10)
                .ToListAsync();
            return top10Products;
        }

        public async Task<int> TotalNumberOfProduct()
        {
            var count = await _context.Products.CountAsync();
            return count;
        }

    }
}
