using Microsoft.AspNetCore.Mvc;
using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Inventory_Mgmt_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            return Ok(result);
        }

        [Authorize]

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartmentById(string id)
        {
            var result = await _departmentService.DeleteDepartmentAsync(Guid.Parse(id));
            return Ok(result);
        }
        [Authorize]

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartmentById(string id, DepartmentDto dto)
        {
            var departmentId = Guid.Parse(id);

            var existingDepartment = await _departmentService.GetByIdAsync(departmentId);
            if (existingDepartment == null)
            {
                return NotFound(new { message = "Department not found" });
            }

            var duplicateNameDepartment = await _departmentService.GetByNameAsync(dto.Name);
            if (duplicateNameDepartment != null && duplicateNameDepartment.Id != departmentId)
            {
                return BadRequest(new { message = "Department name already exists" });
            }

            var updated = await _departmentService.UpdateDepartmentAsync(departmentId, dto);
            return Ok(new { success = true, data = updated });
        }

    }
}
