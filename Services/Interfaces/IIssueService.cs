using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory_Mgmt_System.Services.Interfaces
{
    public interface IIssueService
    {
        Task<ProductIssue> IssueProductsAsync(Guid departmentId, Guid issuedById, List<IssueItemDto> items);
        Task<ProductIssue?> GetIssueByIdAsync(Guid id);
        Task<IEnumerable<ProductIssue>> GetAllIssuesAsync();
        Task CompleteIssueAsync(Guid issueId);
    }
}