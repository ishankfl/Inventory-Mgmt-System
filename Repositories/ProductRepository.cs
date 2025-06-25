using Inventory_Mgmt_System.Data;
using Inventory_Mgmt_System.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Inventory_Mgmt_System.Repositories.Interfaces;

namespace Inventory_Mgmt_System.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Product> CreateProduct(Product product)
        {
            var added = await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return added.Entity;
        }

        public async Task<Product> GetProductById(Guid id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }

            return product;
        }

        public async Task<Product> GetProductByName(string name)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Name == name);


            return product;
        }

        public async Task<List<Product>> GetAllProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.User)
                .ToListAsync();

            return products ?? new List<Product>();
        }

        public async Task<List<Product>> GetProductsByCategory(Guid categoryId)
        {
            var products = await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Category)
                .Include(p => p.User)
                .ToListAsync();

            return products ?? new List<Product>();
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
            existingProduct.Quantity= updatedProduct.Quantity;
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
