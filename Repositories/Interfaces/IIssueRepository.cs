using Inventory_Mgmt_System.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory_Mgmt_System.Repositories.Interfaces
{
    public interface IIssueRepository
    {
        Task<ProductIssue> CreateIssueAsync(ProductIssue issue);
        Task<bool> HasActiveIssueAsync(Guid departmentId);
        Task<ProductIssue?> GetIssueByIdAsync(Guid id);
        Task<IEnumerable<ProductIssue>> GetAllIssuesAsync();
        Task<ProductIssue?> GetLatestUncompletedIssueByDepartmentAsync(Guid departmentId);
        Task<ProductIssue?> GetIssuesByDepartmentId(string departmentId);
        Task<ProductIssue> RemoveItemFromIssue(Guid issueId, ProductIssue product);

        Task<ProductIssue> MakeCompleteIssue(Guid issueId);
        Task<List<Product>> GetTopIssuedProductsAsync();
    }
}