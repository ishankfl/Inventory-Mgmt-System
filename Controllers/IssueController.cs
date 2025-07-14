using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Services;
using Inventory_Mgmt_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.DependencyResolver;
using System;

namespace Inventory_Mgmt_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class IssueController : ControllerBase
    {
        private readonly IIssueService _issueService;

        public IssueController(IIssueService issueService)
        {
            _issueService = issueService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateIssue([FromBody] IssueDto issue)
        {
            try
            {
                if (issue == null)
                    return BadRequest("Issue is null");

                var createdIssue = await _issueService.CreateIssueAsync(issue);
                return CreatedAtAction(nameof(GetIssueById), new { id = createdIssue.Id }, createdIssue);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetIssueById(Guid id)
        {
            try
            {
                var issue = await _issueService.GetIssueByIdAsync(id);
                if (issue == null)
                    return NotFound();

                return Ok(issue);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllIssues()
        {
            try
            {
                var issues = await _issueService.GetAllIssuesAsync();
                return Ok(issues);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIssue(Guid id, [FromBody] IssueDto issue)
        {
            try
            {
                issue.IssueId = id.ToString();
            
                var updatedIssue = await _issueService.UpdateIssueAsync(issue);
                if (updatedIssue == null)
                    return NotFound();

                return Ok(updatedIssue);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIssue(Guid id)
        {
            try
            {
                var deleted = await _issueService.DeleteIssueAsync(id);
                if (!deleted)
                    return NotFound();

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("paginated")]
        public async Task<IActionResult> GetPaginatedIssues([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            try
            {
                var (issues, totalCount) = await _issueService.GetAllPaginatedIssuesAsync(page, limit);

                var response = new
                {
                    Data = issues,
                    Pagination = new
                    {
                        CurrentPage = page,
                        PageSize = limit,
                        TotalCount = totalCount,
                        TotalPages = (int)Math.Ceiling(totalCount / (double)limit)
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
