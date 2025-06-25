using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Inventory_Mgmt_System.Services.Interfaces;

namespace Inventory_Mgmt_System.Services
{
    public class DashboardService : IDashboardService
    {
        private IProductRepository _productRepository;
        public DashboardService(IProductRepository productRepository) { 
            _productRepository = productRepository;
        }
        public async Task<List<Product>> GetTopTenQtyProducts()
        {
            var top10Proudcts = await _productRepository.GetTopTenProductsByQty();
            return top10Proudcts;
        }

    }
}
