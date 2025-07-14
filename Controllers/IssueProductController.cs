using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Inventory_Mgmt_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System;
using System.Threading.Tasks;

namespace Inventory_Mgmt_System.Controllers
{
    [ApiController]
    [Route("api/issues")]
    [Authorize]

    /*  [Authorize(Roles = "Admin,Staff")] */
    public class IssueProductController : ControllerBase
    {
        private readonly IIssueProductService _issueService;

        public IssueProductController(IIssueProductService issueService)
        {
            _issueService = issueService;
        }

        [HttpPost("OneProduct")]
        public async Task<IActionResult> CreateIssue([FromBody] IssueRequestDtoOne request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                IssueItemDto item = new IssueItemDto
                {
                    ProductId = request.ProductId,
                    QuantityIssued = request.QuantityIssued,
                };
                var result = await _issueService.IssueProductOneAsync(
                  request.DepartmentId,
                  request.IssuedById,
                  item);

                return StatusCode(200, new { data = result });

            }

            catch (Exception ex)
            {
                return StatusCode(500, "Something went wrong");
            }

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


        [HttpGet("deptId/{departmentId}")]
        public async Task<IActionResult> GetIssuesByDepartmentId(string departmentId)
        {
            try
            {

                var result = await _issueService.GetIssuesByDepartmentId(departmentId);
                if (result == null)
                {
                    return StatusCode(200, "[]");
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, "{'error': 'Something went wrong'}");
            }
        }

        [Authorize]
        [HttpDelete("removeItem/{issueId}/Product/{productId}")]
        public async Task<IActionResult> RemoveItemFromIssue(string issueId, string productId)
        {
            try
            {
                var result = await _issueService.RemoveItemFromIssue(issueId, productId);
                return Ok(new { success = true, message = "Item removed from issue", updated = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("CompleteIssue/{issueId}")]
        public async Task<IActionResult> MakeCompleteIssue(string issueId)
        {
            try
            {
                var issue = await _issueService.MakeCompleteIssue(Guid.Parse(issueId));

                if (issue == null)
                {
                    return NotFound(new { message = "Issue not found." });
                }

                return Ok(new { message = "Successfully completed issue." });
            }
            catch (Exception e)
            {
                // Log the exception 'e' here as needed
                return StatusCode(500, new { message = "An error occurred while completing the issue." });
            }
        }

        [Authorize]
        [HttpGet("{IssueId}/{ProductId}/{newQuantity}")]
        public async Task<IActionResult> UpdateProduct(string issueId, string productId, string newQuantity)
        {
            try
            {
                var product = await _issueService.UpdateOneProductQty(Guid.Parse(issueId), Guid.Parse(productId),
                   int.Parse(newQuantity));
                return StatusCode(200, product);
            }
            catch(Exception e)
            {
                return StatusCode(500, new { success = false, message = $"Some thing wend wrong {e}" });
            }

        }
    }
}