using Inventory_Mgmt_System.Data;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Inventory_Mgmt_System.Services;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Mgmt_System.Repositories
{
    public class IssueRepository : IIssueRepository
    {
        private readonly AppDbContext _context;

        public IssueRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductIssue> CreateIssueAsync(ProductIssue issue)
        {
            await _context.ProductIssues.AddAsync(issue);
            await _context.SaveChangesAsync();
            return issue;
        }

        public async Task<bool> HasActiveIssueAsync(Guid departmentId)
        {
            return await _context.ProductIssues
                .AnyAsync(i => i.DepartmentId == departmentId && !i.IsCompleted);
        }

        public async Task<ProductIssue?> GetIssueByIdAsync(Guid id)
        {
            return await _context.ProductIssues
                .Include(i => i.Department)
                .Include(i => i.IssuedBy)
                .Include(i => i.IssueItems)
                    .ThenInclude(ii => ii.Product)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<ProductIssue>> GetAllIssuesAsync()
        {
            return await _context.ProductIssues
                .Include(i => i.Department)
                .Include(i => i.IssuedBy)
                .Include(i => i.IssueItems)
                    .ThenInclude(ii => ii.Product)
                .OrderByDescending(i => i.IssueDate)
                .ToListAsync();
        }

        public async Task<ProductIssue?> GetLatestUncompletedIssueByDepartmentAsync(Guid departmentId)
        {
            return await _context.ProductIssues
                .Where(i => i.DepartmentId == departmentId && !i.IsCompleted)
                .OrderByDescending(i => i.IssueDate)
                .FirstOrDefaultAsync();
        }

        public Task<bool> CheckIssueCompletedOrNot(Guid departmentId)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductIssue?> GetIssuesByDepartmentId(string departmentId)
        {
            Guid departmentGuid = Guid.Parse(departmentId);

            var latestProduct = await _context.ProductIssues
                .Where(issue => issue.DepartmentId == departmentGuid).
                Where(issue => issue.IsCompleted == false && !issue.IsCompleted)
                .Include(issue => issue.Department)
                .Include(issue => issue.IssuedBy)
                .Include(issue => issue.IssueItems).Include(i => i.IssueItems).ThenInclude(ii => ii.Product)
                .OrderByDescending(issue => issue.IssueDate) // if you want latest
                .FirstOrDefaultAsync(); // 👈 Fetch one

            return latestProduct;
        }
        public async Task<ProductIssue?> MakeCompleteIssue(Guid issueId)
        {
            var issue = await _context.ProductIssues.FirstOrDefaultAsync(item => item.Id == issueId);

            if (issue == null)
            {
                return null; // or throw an exception, depending on your use case
            }

            issue.IsCompleted = true;

            _context.ProductIssues.Update(issue);
            await _context.SaveChangesAsync();

            return issue;
        }

        // Remove item from issue
        public async Task<ProductIssue> RemoveItemFromIssue(Guid issueId, ProductIssue product)
        {
            var issue = await _context.ProductIssues
                .Include(i => i.IssueItems)
                .FirstOrDefaultAsync(i => i.Id == issueId);

            if (issue == null)
                throw new Exception("Issue not found");

            var productId = product.IssueItems.FirstOrDefault()?.ProductId;

            if (productId == null)
                throw new Exception("ProductId not found in input");

            var itemToRemove = issue.IssueItems
                .FirstOrDefault(i => i.ProductId == productId);

            if (itemToRemove == null)
                throw new Exception("Product not found in issue");

            issue.IssueItems.Remove(itemToRemove);
            _context.IssueItems.Remove(itemToRemove);

            return issue;
        }

        public async Task<List<Product>> GetTopIssuedProductsAsync()
        {
            var result = await _context.IssueItems
                .GroupBy(pi => pi.Product.Id)
                .Select(group => new
                {
                    ProductId = group.Key,
                    TotalIssued = group.Sum(pi => pi.QuantityIssued)
                })
                .OrderByDescending(x => x.TotalIssued)
                .Take(10)
                .Join(_context.Products,
                      g => g.ProductId,
                      p => p.Id,
                      (g, p) => new Product
                      {
                          Id = p.Id,
                          Name = p.Name,
                          Description = p.Description,
                          Price = p.Price,
                          Quantity = g.TotalIssued
                      })
                .ToListAsync();

            return result;
        }

        public async Task<Product> UpdateOneProductQty(Guid issuedId, Guid productId, int newQty)
        {
            var issue = await _context.ProductIssues
                .Include(pi => pi.IssueItems)
                    .ThenInclude(ii => ii.Product) // make sure Product is eagerly loaded
                .FirstOrDefaultAsync(pi => pi.Id == issuedId);

            if (issue == null)
                throw new Exception("ProductIssue not found");

            var issueItem = issue.IssueItems
                .FirstOrDefault(item => item.Product.Id == productId);

            if (issueItem == null)
                throw new Exception("Product not found in issue");

            issueItem.QuantityIssued = newQty;

            await _context.SaveChangesAsync();

            return issueItem.Product; // returning the updated product
        }



    }
}