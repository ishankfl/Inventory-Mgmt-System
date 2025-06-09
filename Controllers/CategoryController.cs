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
        public async Task<IActionResult> AddCategory(Category category)
        {
            var createdCategory = await _categoryService.CreateCategory(category);
            return Ok(createdCategory);
        }

    }
}
