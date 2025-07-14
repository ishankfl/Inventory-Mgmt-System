using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;

namespace Inventory_Mgmt_System.Repositories.Interfaces
{
    public interface IIssueRepository
    {
        Task<Issue> CreateIssueAsync(Issue issue);
        Task<Issue> GetIssueByIdAsync(Guid id);
        Task<IEnumerable<Issue>> GetAllIssuesAsync();
        Task<Issue> UpdateIssueAsync(Issue issue);
        Task<bool> DeleteIssueAsync(Guid id);

         Task<(List<Issue> Issues, int TotalCount)> GetAllPaginatedIssuesAsync(int page, int limit);
       // Task<List<(Item Item, decimal TotalIssuedQuantity)>> GetTopIssuedItemsAsync();
        Task<List<TopIssuedItemResponseDto>> GetTopIssuedItemsAsync();
    }
}
