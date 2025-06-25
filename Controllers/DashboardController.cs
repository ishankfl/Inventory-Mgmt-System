using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Services;
using Inventory_Mgmt_System.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Mgmt_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<IActionResult> GetTopIssuedProductsAsync()
        {
            try
            {
                var topProducts = await _dashboardService.GetTopIssuedProductsAsync();
                return   StatusCode(200, topProducts);
                  //  topProducts;
            }
            catch (Exception ex) { 
                Console.WriteLine(ex);
                return StatusCode(500,new {success="False",message = $"Something went wrong {ex}" });
            }
        }
    }
}
