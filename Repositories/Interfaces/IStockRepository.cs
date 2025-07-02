using Inventory_Mgmt_System.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory_Mgmt_System.Repositories.Interfaces
{
    public interface IStockRepository
    {
        Task<List<Stock>> GetAllStocksAsync();
        Task<Stock> GetStockByIdAsync(Guid id);
        Task<Stock> GetStockByItemIdAsync(Guid itemId);
        Task<Stock> AddStockAsync(Stock stock);
        Task<Stock> UpdateStockAsync(Stock stock);
        Task<bool> DeleteStockAsync(Guid id);
        Task<decimal> GetCurrentQuantityByItemIdAsync(Guid itemId);
    }
}