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
    public class VendorController : ControllerBase
    {
        private readonly IVendorService _vendorService;

        public VendorController(IVendorService vendorService)
        {
            _vendorService = vendorService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Vendor>> GetVendorById(Guid id)
        {
            try
            {
                var vendor = await _vendorService.GetVendorByIdAsync(id);
                return Ok(vendor);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Vendor>> AddVendor([FromBody] Vendor vendor)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = GetUserId();
                var createdVendor = await _vendorService.AddVendorAsync(vendor, userId);
                return CreatedAtAction(nameof(GetVendorById), new { id = createdVendor.Id }, createdVendor);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Vendor>> DeleteVendor(Guid id)
        {
            try
            {
                var userId = GetUserId();
                var deletedVendor = await _vendorService.DeleteVendorAsync(id, userId);
                return Ok(deletedVendor);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetVendors([FromQuery] string searchTerm = "", [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var (vendors, totalCount) = await _vendorService.SearchVendorsAsync(searchTerm, pageNumber, pageSize);
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var result = new
                {
                    Data = vendors,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages
                };

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving vendors.");
            }
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst("id")?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("User ID is missing or invalid.");

            return userId;
        }
    }
}
