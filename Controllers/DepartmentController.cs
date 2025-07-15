using Microsoft.AspNetCore.Mvc;
using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("User ID claim is missing or invalid.");
            }
            return userId;
        }

        // POST: api/Department
        [HttpPost]
        public async Task<IActionResult> AddDepartment([FromBody] DepartmentDto department)
        {
            try
            {
                var created = await _departmentService.CreateDepartmentAsync(department, GetUserId());
                return CreatedAtAction(nameof(GetDepartmentById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // GET: api/Department
        [HttpGet]
        public async Task<IActionResult> DepartmentView()
        {
            var result = await _departmentService.GetAllDepartmentsAsync();
            return Ok(result);
        }

        // GET: api/Department/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentById(string id)
        {
            if (!Guid.TryParse(id, out var guidId))
                return BadRequest(new { message = "Invalid department ID." });

            var result = await _departmentService.GetDepartmentByIdAsync(guidId);
            if (result == null)
                return NotFound(new { message = "Department not found" });

            return Ok(result);
        }

        // DELETE: api/Department/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartmentById(string id)
        {
            if (!Guid.TryParse(id, out var guidId))
                return BadRequest(new { message = "Invalid department ID." });

            var deleted = await _departmentService.DeleteDepartmentAsync(guidId, GetUserId());
            if (!deleted)
                return NotFound(new { message = "Department not found" });

            return Ok(new { success = true });
        }

        // PUT: api/Department/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartmentById(string id, [FromBody] DepartmentDto dto)
        {
            if (!Guid.TryParse(id, out var guidId))
                return BadRequest(new { message = "Invalid department ID." });

            var updated = await _departmentService.UpdateDepartmentAsync(guidId, dto, GetUserId());
            if (updated == null)
                return BadRequest(new { message = "Update failed. Department not found or name already exists." });

            return Ok(new { success = true, data = updated });
        }
    }
}
