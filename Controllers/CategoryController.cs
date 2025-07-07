using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Mgmt_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryCreateDto categorydto)
        {
            try
            {
                var createdCategory = await _categoryService.CreateCategory(categorydto);
                return Ok(createdCategory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Something went wrong: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategory()
        {
            try
            {
                var categories = await _categoryService.GetAllCategories();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Something went wrong: {ex.Message}");
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            try
            {
                var category = await _categoryService.DeleteCategory(id);
                return Ok(category);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Something went wrong: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            try
            {
                var category = await _categoryService.GetCategoryById(id);
                if (category == null)
                {
                    return NotFound(new { message = $"Category with ID {id} not found." });
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Something went wrong: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, CategoryCreateDto categoryDto)
        {
            try
            {
                var updatedCategory = await _categoryService.UpdateCategory(id, categoryDto);
                return Ok(updatedCategory);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(409, new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Something went wrong: {ex.Message}");
            }
        }
    }
}
