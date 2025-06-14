using Microsoft.AspNetCore.Mvc;
using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Services.Interfaces;

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


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartmentById(string id)
        {
            var result = await _departmentService.DeleteDepartmentAsync(Guid.Parse(id));
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartmentById(string id, DepartmentDto dto)
        {
        /*    Department department = new Department
            {

                Description = dto.Description,
                Name = dto.Name,
                Id = dto.Id
            };*/
            var result = await _departmentService.UpdateDepartmentAsync(Guid.Parse(id), dto);
            return Ok(result);
        }
    }
}
