using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Inventory_Mgmt_System.Services.Interfaces;

namespace Inventory_Mgmt_System.Services
{
    public class DashboardService : IDashboardService
    {
        private IProductRepository _productRepository;
        private readonly IIssueRepository _issueRepository;
        public DashboardService(IProductRepository productRepository, IIssueRepository issueRepository) { 
            _productRepository = productRepository;
            _issueRepository = issueRepository;
        }
        public async Task<List<Product>> GetTopTenQtyProducts()
        {
            var top10Proudcts = await _productRepository.GetTopTenProductsByQty();
            return top10Proudcts;
        }

        public async Task<List<Product>> GetTopIssuedProductsAsync()
        {
            var topIssuedProducts = await _issueRepository.GetTopIssuedProductsAsync();
            return topIssuedProducts;
        }


    }

}
