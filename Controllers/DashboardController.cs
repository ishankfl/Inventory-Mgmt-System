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
        [HttpGet("TopProductByQty")]
        public async Task<IActionResult> TopProductByQty()
        {
            try
            {

                var top10Products = await _dashboardService.GetTopTenQtyProducts();
                return StatusCode(200, top10Products);
            }
            catch (Exception ex) { 
                return StatusCode(500, ex);
           
            
            }
        }
        [HttpGet("GetTopIssuedProducts")]
        public async Task<IActionResult> GetTopIssuedItemsAsync()
        {
            try
            {
                var topProducts = await _dashboardService.GetTopIssuedItemsAsync();
                Console.WriteLine(topProducts);
                return    Ok(topProducts);
                  //  topProducts;
            }
            catch (Exception ex) { 
                Console.WriteLine(ex);
                throw;
            }
        }

        [HttpGet("GetCount")]
        public async Task<IActionResult> GetCount()
        {
            try
            {

                var getCount = await _dashboardService.GetCount();
                return StatusCode(200, getCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { success = "False", message = "SOmething went wrong" });
            }
        }
    }
}
