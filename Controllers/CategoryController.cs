using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Mgmt_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            this._categoryService = categoryService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryCreateDto categorydto)
        {
            Category category = new Category
            {
                Id = Guid.NewGuid(),
                Name = categorydto.Name,
                UserId = categorydto.UserId,
                Description = categorydto.Description,
            };
            try
            {
                var isExistCategory = await _categoryService.GetCategoryByName(categorydto.Name);
                if (isExistCategory != null)
                {
                    return StatusCode(409, $"Category with this name already exist:");

                }
                var createdCategory = await _categoryService.CreateCategory(category);
                return Ok(createdCategory);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Something went wrong {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategory()
        {
            var categories = await _categoryService.GetAllCategories();
            return Ok(categories);
        }



        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var categories = await _categoryService.DeleteCategory(id);
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var categories = await _categoryService.GetCategoryById(id);
            return Ok(categories);
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, CategoryCreateDto category)
        {
            var categories = await _categoryService.GetCategoryById(id);
            var categoryByName = await _categoryService.GetCategoryByName(category.Name);
            
            if (categoryByName != null && category.Name != categories.Name)
            {
                return StatusCode(409, new { message = "Category with this name already exist" });
            }
            categories.setName(category.Name);
            categories.Description = category.Description;

            var updatedCategory = await _categoryService.UpdateCategory(categories);
            return Ok(updatedCategory);
        }

    }
}
