using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory_Mgmt_System.Services.Interfaces
{
    public interface IIssueProductService
    {
        Task<ProductIssue> IssueProductsAsync(Guid departmentId, Guid issuedById, List<IssueItemDto> items);
        Task<ProductIssue?> GetIssueByIdAsync(Guid id);
        Task<IEnumerable<ProductIssue>> GetAllIssuesAsync();
        Task CompleteIssueAsync(Guid issueId);
        Task<ProductIssue> IssueProductOneAsync(Guid departmentId, Guid issuedById, IssueItemDto item);

        Task<ProductIssue?> GetIssuesByDepartmentId(string departmentId);
        Task<ProductIssue> RemoveItemFromIssue(string issueId, string productId);
        Task<ProductIssue> MakeCompleteIssue(Guid issueId);
        Task<Product> UpdateOneProductQty(Guid issuedId, Guid productId, int newQty);

    }
}