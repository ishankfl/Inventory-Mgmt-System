using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Inventory_Mgmt_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReceiptsController : ControllerBase
    {
        private readonly IReceiptService _receiptService;

        public ReceiptsController(IReceiptService receiptService)
        {
            _receiptService = receiptService;
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst("id")?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim))
                throw new UnauthorizedAccessException("User ID is missing or invalid.");

            return Guid.Parse(userIdClaim);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReceipt([FromBody] ReceiptCreateDto receiptDto)
        {
            try
            {
                var receipt = await _receiptService.CreateReceiptAsync(receiptDto, GetUserId());
                return Ok(new { success = true, data = receipt });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReceipt(Guid id)
        {
            try
            {
                var receipt = await _receiptService.GetReceiptByIdAsync(id);
                if (receipt == null)
                    return NotFound(new { success = false, message = "Receipt not found" });

                return Ok(new { success = true, data = receipt });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllReceipts()
        {
            try
            {
                var receipts = await _receiptService.GetAllReceiptsAsync();
                return Ok(new { success = true, data = receipts });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("paginate")]
        public async Task<IActionResult> GetAllReceiptsPaginated([FromQuery] int page = 1, [FromQuery] int limit = 6)
        {
            try
            {
                var (receipts, totalCount) = await _receiptService.GetAllReceiptsPaginatedAsync(page, limit);
                var totalPages = (int)Math.Ceiling((double)totalCount / limit);

                return Ok(new
                {
                    success = true,
                    data = receipts,
                    pagination = new
                    {
                        currentPage = page,
                        totalCount,
                        totalPages,
                        pageSize = limit
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReceipt(Guid id, [FromBody] ReceiptUpdateDto receiptDto)
        {
            try
            {
                var updatedReceipt = await _receiptService.UpdateReceiptAsync(id, receiptDto, GetUserId());
                return Ok(new { success = true, data = updatedReceipt });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while updating the receipt." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReceipt(Guid id)
        {
            try
            {
                var result = await _receiptService.DeleteReceiptAsync(id, GetUserId());
                if (!result)
                    return NotFound(new { success = false, message = "Receipt not found" });

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetAllReceiptDetails()
        {
            try
            {
                var details = await _receiptService.GetAllReceiptDetailsAsync();
                return Ok(new { success = true, data = details });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("details/simple")]
        public async Task<IActionResult> GetSimplifiedReceiptDetails()
        {
            try
            {
                var details = await _receiptService.GetSimplifiedReceiptDetailsAsync();
                return Ok(new { success = true, data = details });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("top10qty")]
        public async Task<IActionResult> GetTop10Item()
        {
            try
            {
                var details = await _receiptService.GetTop10Item();
                return Ok(new { success = true, data = details });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
