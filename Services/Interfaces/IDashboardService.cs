using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;

namespace Inventory_Mgmt_System.Services.Interfaces
{
    public interface IDashboardService
    {
      //  Task<List<Product>> GetTopTenQtyProducts();
//        Task<List<Product>> GetTopIssuedProductsAsync();
        Task<Dictionary<string,int>> GetCount();
        // Task<List<TopItemDto>> GetTopIssuedProductsAsync();

        //Task<IEnumerable<(Item Item, decimal TotalIssuedQuantity)>> GetTopIssuedItemsAsync();
        // Task<List<(Item Item, decimal TotalIssuedQuantity)>> GetTopIssuedItemsAsync();
        Task<List<TopIssuedItemResponseDto>> GetTopIssuedItemsAsync();
        Task<List<TopItemDto>> GetTopTenQtyProducts();

    }
}
