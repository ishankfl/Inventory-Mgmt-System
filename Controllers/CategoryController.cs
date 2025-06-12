using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Mgmt_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService) { 
            this._categoryService = categoryService;
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryCreateDto categorydto)
        {
            Category category = new Category
            {
                Name = categorydto.Name,
                UserId = categorydto.UserId,
                Description = categorydto.Description,
            };
            var createdCategory = await _categoryService.CreateCategory(category);
            return Ok(createdCategory);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategory()
        {
            var categories = await _categoryService.GetAllCategories();
            return Ok(categories);
        }


        [HttpPut]
        public async Task<IActionResult> UpdateCategory()
        {
            var categories = await _categoryService.GetAllCategories();
            return Ok(categories);
        }


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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, CategoryCreateDto category)
        {
            var categories = await _categoryService.GetCategoryById(id);
            categories.setName(category.Name);
            categories.Description = category.Description;

            var updatedCategory = await _categoryService.UpdateCategory(categories);
            return Ok(updatedCategory);
        }

    }
}
