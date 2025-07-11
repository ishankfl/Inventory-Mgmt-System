using System;
using System.Threading.Tasks;
using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Mgmt_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        // GET: api/Item
        [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            try
            {

            var items = await _itemService.GetAllItemsAsync();
            return Ok(items);
            }
            catch(Exception ex) {
            
                return BadRequest(ex.ToString());
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

        // POST: api/Item
        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] ItemDto item)
        {
            try
            {
                var newItem = await _itemService.AddItemAsync(item);
                return CreatedAtAction(nameof(GetItemById), new { id = newItem.Id }, newItem);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Item/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(string idInString, [FromBody] ItemDto item)
        {
            Guid id = Guid.Parse(idInString);
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(new { message = "ID in URL and body do not match." });

                var updatedItem = await _itemService.UpdateItemAsync(id, item);
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
                var deletedItem = await _itemService.DeleteItemAsync(id);
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
        }
    }
}
