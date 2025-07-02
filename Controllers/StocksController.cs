using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Inventory_Mgmt_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly IStockService _stockService;
        private readonly ILogger<StocksController> _logger;

        public StocksController(IStockService stockService, ILogger<StocksController> logger)
        {
            _stockService = stockService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Stock>>> GetAllStocks()
        {
            try
            {
                var stocks = await _stockService.GetAllStocksAsync();
                return Ok(stocks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all stocks");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Stock>> GetStockById(Guid id)
        {
            try
            {
                var stock = await _stockService.GetStockByIdAsync(id);
                if (stock == null)
                {
                    return NotFound();
                }
                return Ok(stock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting stock with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("item/{itemId}")]
        public async Task<ActionResult<Stock>> GetStockByItemId(Guid itemId)
        {
            try
            {
                var stock = await _stockService.GetStockByItemIdAsync(itemId);
                if (stock == null)
                {
                    return NotFound();
                }
                return Ok(stock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting stock for item ID: {itemId}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Stock>> CreateStock([FromBody] Stock stock)
        {
            try
            {
                var createdStock = await _stockService.AddStockAsync(stock);
                return CreatedAtAction(nameof(GetStockById), new { id = createdStock.Id }, createdStock);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating stock");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStock(Guid id, [FromBody] Stock stock)
        {
            try
            {
                if (id != stock.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var updatedStock = await _stockService.UpdateStockAsync(stock);
                return Ok(updatedStock);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating stock with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStock(Guid id)
        {
            try
            {
                var result = await _stockService.DeleteStockAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting stock with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("quantity/{itemId}")]
        public async Task<ActionResult<decimal>> GetCurrentQuantity(Guid itemId)
        {
            try
            {
                var quantity = await _stockService.GetCurrentQuantityByItemIdAsync(itemId);
                return Ok(quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting quantity for item ID: {itemId}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("update-quantity/{itemId}")]
        public async Task<ActionResult<Stock>> UpdateStockQuantity(Guid itemId, [FromBody] decimal quantityChange)
        {
            try
            {
                var updatedStock = await _stockService.UpdateStockQuantityAsync(itemId, quantityChange);
                return Ok(updatedStock);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating quantity for item ID: {itemId}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}