using Inventory_Mgmt_System.Data;
using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Inventory_Mgmt_System.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Mgmt_System.Services
{
    public class IssueService : IIssueService
    {
        private readonly IIssueRepository _issueRepository;
        private readonly IProductRepository _productRepository;
        private readonly AppDbContext _context;

        public IssueService(
            IIssueRepository issueRepository,
            IProductRepository productRepository,
            AppDbContext context)
        {
            _issueRepository = issueRepository;
            _productRepository = productRepository;
            _context = context;
        }

        public async Task<ProductIssue> IssueProductsAsync(Guid departmentId, Guid issuedById, List<IssueItemDto> items)
        {
            if (items == null || !items.Any())
                throw new ArgumentException("No items to issue.");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Get or create the active issue for this department
                var activeIssue = await GetOrCreateActiveIssue(departmentId, issuedById);

                // Process each item
                foreach (var item in items)
                {
                    await ProcessIssueItem(activeIssue, item);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await _issueRepository.GetIssueByIdAsync(activeIssue.Id);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ProductIssue> IssueProductOneAsync(Guid departmentId, Guid issuedById, IssueItemDto item)
        {
            if (item == null)
                throw new ArgumentException("No items to issue.");

         //   await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Get or create the active issue for this department
                var activeIssue = await GetOrCreateActiveIssue(departmentId, issuedById);

                // Process each item
               /* foreach (var item in items)
                {*/
                    await ProcessIssueItem(activeIssue, item);
                

                await _context.SaveChangesAsync();
              //  await transaction.CommitAsync();

                return await _issueRepository.GetIssueByIdAsync(activeIssue.Id);
            }
            catch
            {
                //await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task<ProductIssue> GetOrCreateActiveIssue(Guid departmentId, Guid issuedById)
        {
            var activeIssue = await _issueRepository.GetLatestUncompletedIssueByDepartmentAsync(departmentId);

            if (activeIssue == null)
            {
                activeIssue = new ProductIssue
                {
                    Id = Guid.NewGuid(),
                    DepartmentId = departmentId,
                    IssuedById = issuedById,
                    IssueDate = DateTime.UtcNow,
                    IsCompleted = false
                };

                await _issueRepository.CreateIssueAsync(activeIssue);
            }

            return activeIssue;
        }

        private async Task ProcessIssueItem(ProductIssue issue, IssueItemDto item)
        {
            var product = await _productRepository.GetProductById(item.ProductId);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {item.ProductId} not found.");

            if (product.Quantity < item.QuantityIssued)
                throw new InvalidOperationException(
                    $"Insufficient quantity for product '{product.Name}'. Available: {product.Quantity}, Requested: {item.QuantityIssued}");

            // Check if item already exists in the uncompleted issue
            var existingItem = await _context.IssueItems
                .FirstOrDefaultAsync(ii => ii.ProductIssueId == issue.Id && ii.ProductId == item.ProductId);

            if (existingItem != null)
            {
                // Update existing item
                existingItem.QuantityIssued += item.QuantityIssued;
            }
            else
            {
                // Create new issue item
                var issueItem = new IssueItem
                {
                    Id = Guid.NewGuid(),
                    ProductIssueId = issue.Id,
                    ProductId = item.ProductId,
                    QuantityIssued = item.QuantityIssued
                };

                _context.IssueItems.Add(issueItem);
            }

            // Deduct stock
            product.Quantity -= item.QuantityIssued;
        }


        public async Task<ProductIssue?> GetIssueByIdAsync(Guid id)
        {
            return await _issueRepository.GetIssueByIdAsync(id);
        }

        public async Task<IEnumerable<ProductIssue>> GetAllIssuesAsync()
        {
            return await _issueRepository.GetAllIssuesAsync();
        }

        public async Task CompleteIssueAsync(Guid issueId)
        {
            var issue = await _issueRepository.GetIssueByIdAsync(issueId);
            if (issue == null)
                throw new KeyNotFoundException($"Issue with ID {issueId} not found");

            issue.IsCompleted = true;
            await _context.SaveChangesAsync();
        }

        public async Task<ProductIssue> GetIssuesByDepartmentId(string departmentId)
        {
            var productIssue = await _issueRepository.GetIssuesByDepartmentId(departmentId);
            return productIssue;
        }
        public async Task<ProductIssue> RemoveItemFromIssue(string issueId, string productId)
        {
            var issueGuid = Guid.Parse(issueId);
            var productGuid = Guid.Parse(productId);

            // Get the full issue with its items
            var issue = await GetIssueByIdAsync(issueGuid);
            if (issue == null)
                throw new Exception("Issue not found");

            // Find the issue item to be removed
            var itemToRemove = issue.IssueItems.FirstOrDefault(i => i.ProductId == productGuid);
            if (itemToRemove == null)
                throw new Exception("Product not found in issue");

            // Remove item from issue and database
            issue.IssueItems.Remove(itemToRemove);
            _context.IssueItems.Remove(itemToRemove); // or _issueRepository

            // Get the product to update quantity
            var product = await _productRepository.GetProductById(productGuid);
            if (product == null)
                throw new Exception("Product not found");

            // Add the quantity back to product stock
            product.Quantity += itemToRemove.QuantityIssued;

            // Update the product
           await  _productRepository.UpdateProduct(product);
            await _context.SaveChangesAsync();
            // Save changes
            return issue;
        }


    }
}