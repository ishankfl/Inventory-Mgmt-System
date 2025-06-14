using Inventory_Mgmt_System.Data;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
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
    }
}