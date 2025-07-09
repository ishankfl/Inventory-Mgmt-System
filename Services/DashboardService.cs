using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Inventory_Mgmt_System.Services.Interfaces;

namespace Inventory_Mgmt_System.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IProductRepository _productRepository;
        private readonly IIssueProductRepository _issueRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IItemRepository _itemRepository;
        public DashboardService(IProductRepository productRepository, IIssueProductRepository issueRepository,
            IDepartmentRepository departmentRepository,
            ICategoryRepository categoryRepository,
            IUserRepository userRepository,
            IItemRepository itemRepository

            )
        {
            _productRepository = productRepository;
            _issueRepository = issueRepository;
            _departmentRepository = departmentRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
            _itemRepository = itemRepository;
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

        public async Task<Dictionary<string, int>> GetCount()
        {
            int userCount = await _userRepository.TotalNumberOfUser();
            int productCount = await _itemRepository. GetTotalCountOfItems();
            int categoryCount = await _categoryRepository.TotalNumberOfCategory();
            int departmentCount = await _departmentRepository.TotalNumberOfDepartments();

            var counts = new Dictionary<string, int>
                {
                    { "users", userCount },
                    { "products", productCount },
                    { "categories", categoryCount },
                    { "departments", departmentCount }
                };

            return counts;
        }



    }

}
