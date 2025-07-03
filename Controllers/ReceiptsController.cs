using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Services;
using Inventory_Mgmt_System.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Mgmt_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptsController : ControllerBase
    {
        private readonly IReceiptService _receiptService;

        public ReceiptsController(IReceiptService receiptService)
        {
            _receiptService = receiptService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReceipt([FromBody] ReceiptCreateDto receiptDto)
        {
            try
            {
                var receipt = await _receiptService.CreateReceiptAsync(receiptDto);
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

        [HttpGet]
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReceipt(string id,  ReceiptUpdateDto receiptDto)
        {
            Console.WriteLine(receiptDto);
            try
            {
                var updatedReceipt = await _receiptService.UpdateReceiptAsync(Guid.Parse(id), receiptDto);
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
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReceipt(Guid id)
        {
            try
            {
                var result = await _receiptService.DeleteReceiptAsync(id);
                if (!result)
                    return NotFound(new { success = false, message = "Receipt not found" });

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}