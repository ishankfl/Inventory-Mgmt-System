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

        // GET: api/vendor - Get all vendors
        [HttpGet]
        public async Task<ActionResult<List<Vendor>>> GetAllVendors()
        {
            var vendors = await _vendorService.GetAllVendorsAsync();
            return Ok(vendors);
        }

        // GET: api/vendor/{id} - Get a specific vendor by ID
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

        // POST: api/vendor - Add a new vendor
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

        // DELETE: api/vendor/{id} - Delete a vendor by ID
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

        // Helper to extract user ID from JWT claims
        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("User ID is missing or invalid.");
            }

            return userId;
        }
    }
}
