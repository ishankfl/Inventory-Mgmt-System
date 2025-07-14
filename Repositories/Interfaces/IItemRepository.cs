using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inventory_Mgmt_System.Models;

namespace Inventory_Mgmt_System.Repositories.Interfaces
{
    public interface IItemRepository
    {
        // Task<List<Item>> GetAllAsync();
        Task<(List<Item> Items, int TotalCount)> GetAllPaginatedAsync(int page, int limit);
        Task<Item> GetByIdAsync(Guid id);
        Task<Item> AddAsync(Item item);
        Task<Item> UpdateAsync(Item item);
        Task<Item> DeleteAsync(Guid id);
        Task<bool> Exists(Guid id);

        Task<int> GetTotalCountOfItems();
    }
}
