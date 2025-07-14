using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;

namespace Inventory_Mgmt_System.Services.Interfaces
{
    public interface IIssueService
    {
        Task<Issue> CreateIssueAsync(IssueDto issue);
        Task<Issue> GetIssueByIdAsync(Guid id);
        Task<IEnumerable<Issue>> GetAllIssuesAsync();
        Task<Issue> UpdateIssueAsync(IssueDto issue);
        Task<bool> DeleteIssueAsync(Guid id);

        Task<IEnumerable<(Item Item, decimal TotalIssuedQuantity)>> GetTopIssuedItemsAsync();
        Task<(List<Issue> Issues, int TotalCount)> GetAllPaginatedIssuesAsync(int page, int limit);
    }
}
