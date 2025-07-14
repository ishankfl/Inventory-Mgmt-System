using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Services;
using Inventory_Mgmt_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Mgmt_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        [HttpGet("overview")]
        public async Task<IActionResult> GetDashboardOverview()
        {
            try
            {
                var data = await _dashboardService.GetFullDashboardDataAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { success = false, message = "An error occurred while loading dashboard data." });
            }
        }

    }
}
