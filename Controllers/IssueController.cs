using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Inventory_Mgmt_System.Controllers
{
    [ApiController]
    [Route("api/issues")]
  /*  [Authorize(Roles = "Admin,Staff")] */
    public class IssueController : ControllerBase
    {
        private readonly IIssueService _issueService;

        public IssueController(IIssueService issueService)
        {
            _issueService = issueService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateIssue([FromBody] IssueRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _issueService.IssueProductsAsync(
                    request.DepartmentId,
                    request.IssuedById,
                    request.Items);

                                return StatusCode(200, new { data = result });

            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred." });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetIssue(Guid id)
        {
            try
            {
                var issue = await _issueService.GetIssueByIdAsync(id);
                return issue != null ? Ok(issue) : NotFound();
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Failed to retrieve issue." });
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
            catch (Exception)
            {
                return StatusCode(500, new { error = "Failed to retrieve issues." });
            }
        }

        [HttpPatch("{id:guid}/complete")]
        public async Task<IActionResult> CompleteIssue(Guid id)
        {
            try
            {
                await _issueService.CompleteIssueAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Failed to complete issue." });
            }
        }
    }
}