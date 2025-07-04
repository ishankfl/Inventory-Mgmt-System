using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Services;
using Inventory_Mgmt_System.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Inventory_Mgmt_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IssueController : ControllerBase
    {
        private readonly IIssueService _issueService;

        public IssueController(IIssueService issueService)
        {
            _issueService = issueService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateIssue([FromBody] Issue issue)
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
                // Log exception here if needed
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
                // Log exception here if needed
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
                // Log exception here if needed
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIssue(Guid id, [FromBody] Issue issue)
        {
            try
            {
                if (issue == null || id != issue.Id)
                    return BadRequest("Issue is null or ID mismatch.");

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
                // Log exception here if needed
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
                // Log exception here if needed
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
