using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Inventory_Mgmt_System.Services.Interfaces;

namespace Inventory_Mgmt_System.Services
{
    public class IssueService : IIssueService
    {
        private readonly IIssueRepository _issueRepository;

        public IssueService(IIssueRepository issueRepository)
        {
            _issueRepository = issueRepository;
        }

        public async Task<Issue> CreateIssueAsync(IssueDto issueDto)
        {
            var issue = new Issue
            {
                IssueId = issueDto.IssueId,
                IssueDate = issueDto.IssueDate,
                InvoiceNumber = issueDto.InvoiceNumber,
                InvoiceDate = issueDto.InvoiceDate,
                DeliveryNote = issueDto.DeliveryNote,
                IssuedByUserId = issueDto.IssuedByUserId,
                DepartmentId = issueDto.DepartmentId,
                IssueDetails = issueDto.IssueDetails.Select(d => new IssueDetail
                {
                    Id = Guid.NewGuid(),
                    ItemId = d.ItemId,
                    Quantity = d.Quantity,
                    IssueRate = d.IssueRate
                     
                }).ToList()
            };

            ValidateIssue(issue, isNew: true);

            return await _issueRepository.CreateIssueAsync(issue);
        }


        public async Task<Issue> GetIssueByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid issue ID.");

            return await _issueRepository.GetIssueByIdAsync(id);
        }

        public async Task<IEnumerable<Issue>> GetAllIssuesAsync()
        {
            return await _issueRepository.GetAllIssuesAsync();
        }

        public async Task<Issue> UpdateIssueAsync(IssueDto issueDto)
        {
            Issue issue = new Issue
            {
                DeliveryNote = issueDto.DeliveryNote,
                DepartmentId = issueDto.DepartmentId,
                InvoiceDate = issueDto.InvoiceDate,
                InvoiceNumber = issueDto.InvoiceNumber,
                IssueDate = issueDto.IssueDate,
                IssuedByUserId = issueDto.IssuedByUserId,
                Id = Guid.Parse(issueDto.IssueId),

            };
            issue.IssueDetails = issueDto.IssueDetails.Select(d => new IssueDetail
            {
                Id = Guid.NewGuid(), 
                ItemId = d.ItemId,
                Quantity = d.Quantity,
                IssueRate = d.IssueRate
            }).ToList();

            if (issue.Id == Guid.Empty)
                throw new ArgumentException("Invalid issue ID.");

            ValidateIssue(issue, isNew: false);
            return await _issueRepository.UpdateIssueAsync(issue);
        }
        public async Task<(List<Issue> Issues, int TotalCount)> GetAllPaginatedIssuesAsync(int page, int limit)
        {
            return await _issueRepository.GetAllPaginatedIssuesAsync(page, limit);
        }

        public async Task<bool> DeleteIssueAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid issue ID.");

            return await _issueRepository.DeleteIssueAsync(id);
        }

        private void ValidateIssue(Issue issue, bool isNew)
        {
            if (issue == null)
                throw new ArgumentNullException(nameof(issue), "Issue cannot be null.");

            if (issue.IssueDate == default)
                throw new ArgumentException("Issue date is required.");

            if (string.IsNullOrWhiteSpace(issue.DepartmentId.ToString()))
                throw new ArgumentException("Department is required.");

            if (issue.IssuedByUserId == Guid.Empty)
                throw new ArgumentException("IssuedByUserId is required.");

            if (issue.IssueDetails == null || !issue.IssueDetails.Any())
                throw new ArgumentException("At least one issue detail is required.");

            foreach (var detail in issue.IssueDetails)
            {
                if (detail.ItemId == Guid.Empty)
                    throw new ArgumentException("ItemId is required in issue details.");

                if (detail.Quantity <= 0)
                    throw new ArgumentException("Quantity must be greater than zero in issue details.");
            }

            if (!isNew && issue.Id == Guid.Empty)
                throw new ArgumentException("Issue ID is required for update.");
        }

        public async Task<List<TopIssuedItemResponseDto>> GetTopIssuedItemsAsync()
        {
            return await _issueRepository.GetTopIssuedItemsAsync();
        }
    }
}
