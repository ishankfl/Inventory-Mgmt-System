using Microsoft.AspNetCore.Mvc;
using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Inventory_Mgmt_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddDepartment([FromBody] DepartmentDto department)
        {
            var result = await _departmentService.CreateDepartmentAsync(department);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> DepartmentView()
        {
            var result = await _departmentService.GetAllDepartmentsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentById(string id)
        {
            var result = await _departmentService.GetDepartmentByIdAsync(Guid.Parse(id));
            if (result == null)
                return NotFound(new { message = "Department not found" });

            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartmentById(string id)
        {
            var deleted = await _departmentService.DeleteDepartmentAsync(Guid.Parse(id));
            if (!deleted)
                return NotFound(new { message = "Department not found" });

            return Ok(new { success = true });
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartmentById(string id, DepartmentDto dto)
        {
            var updated = await _departmentService.UpdateDepartmentAsync(Guid.Parse(id), dto);
            if (updated == null)
                return BadRequest(new { message = "Update failed. Department not found or name already exists." });

            return Ok(new { success = true, data = updated });
        }
    }
}
