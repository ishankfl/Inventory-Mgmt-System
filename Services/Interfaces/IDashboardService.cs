using Inventory_Mgmt_System.Models;

namespace Inventory_Mgmt_System.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<List<Product>> GetTopTenQtyProducts();
        Task<List<Product>> GetTopIssuedProductsAsync();
        Task<Dictionary<string,int>> GetCount();

    }
}
