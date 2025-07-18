using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Inventory_Mgmt_System.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory_Mgmt_System.Services
{
    public class DashboardService : IDashboardService
    {
       
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IReceiptService _receiptService;
        private readonly IIssueService _issueService;

        public DashboardService(
           
            IDepartmentRepository departmentRepository,
            ICategoryRepository categoryRepository,
            IUserRepository userRepository,
            IItemRepository itemRepository,
            IReceiptService receiptService,
            IIssueService issueService)
        {
           
            _departmentRepository = departmentRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
            _itemRepository = itemRepository;
            _receiptService = receiptService;
            _issueService = issueService;
        }

        public async Task<List<TopItemDto>> GetTopTenQtyProducts()
        {
            var topProducts = await _receiptService.GetTop10Item();
            return topProducts;
        }

        public async Task<List<TopIssuedItemResponseDto>> GetTopIssuedItemsAsync()
        {
            var issuedItems = await _issueService.GetTopIssuedItemsAsync();
            return issuedItems;
        }

        public async Task<List<DailyTotalPriceDto>> GetDailyTotalReceiptValueAsync()
        {
            return await _receiptService.GetDailyTotalReceiptValueAsync();
        }

        public async Task<Dictionary<string, dynamic>> GetCount()
        {
            var userCount = await _userRepository.TotalNumberOfUser();
            var productCount = await _itemRepository.GetTotalCountOfItems();
            var categoryCount = await _categoryRepository.TotalNumberOfCategory();
            var departmentCount = await _departmentRepository.TotalNumberOfDepartments();

            return new Dictionary<string, dynamic>
            {
                { "users", userCount },
                { "products", productCount },
                { "categories", categoryCount },
                { "departments", departmentCount }
            };
        }

        public async Task<DashboardDataDto> GetFullDashboardDataAsync()
        {
            var topProductsByQty = await GetTopTenQtyProducts();
            var topIssuedItems = await GetTopIssuedItemsAsync();
            var cardCounts = await GetCount();
            var dailyReceiptValues = await GetDailyTotalReceiptValueAsync();

            return new DashboardDataDto
            {
                TopProductsByQty = topProductsByQty,
                TopIssuedItems = topIssuedItems,
                CardCounts = cardCounts,
                DailyTotalReceiptValue = dailyReceiptValues
            };
        }
    }
}
