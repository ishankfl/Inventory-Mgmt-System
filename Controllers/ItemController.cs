using System;
using System.Threading.Tasks;
using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Inventory_Mgmt_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems(
      [FromQuery] int page = 1,
      [FromQuery] int limit = 10,
      [FromQuery] string search = "")
        {
            try
            {
                var (items, totalCount) = await _itemService.GetAllItemsPaginatedAsync(page, limit, search);

                var response = new
                {
                    data = items,
                    total = totalCount,
                    page,
                    limit
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/Item/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemById(Guid id)
        {
            try
            {
                var item = await _itemService.GetItemByIdAsync(id);
                return Ok(item);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("allname")]
        public async Task<IActionResult> GetAllItemNamesAndIdsAsync()
        {
            try
            {
                var items = await _itemService.GetAllItemNamesAndIdsAsync();
                return Ok(items);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.ToString() });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] ItemDto item)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(new { errors });
            }

            try
            {
                var userId = GetUserId();
                var newItem = await _itemService.AddItemAsync(item, userId);
                return CreatedAtAction(nameof(GetItemById), new { id = newItem.Id }, newItem);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // PUT: api/Item/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(string id, [FromBody] ItemDto item)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = GetUserId();
                var updatedItem = await _itemService.UpdateItemAsync(Guid.Parse(id), item, userId);
                return Ok(updatedItem);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Item/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            try
            {
                var userId = GetUserId();
                var deletedItem = await _itemService.DeleteItemAsync(id, userId);
                return Ok(deletedItem);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message }); // 409 Conflict
            }
        }

        // Helper method to get user ID from claims
   private Guid GetUserId()
{
    var userIdClaim = User.FindFirst("id")?.Value;

    if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
    {
        throw new UnauthorizedAccessException("User ID claim is missing or invalid.");
    }

    return userId;
}

    }
}
