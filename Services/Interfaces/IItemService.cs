using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;

namespace Inventory_Mgmt_System.Services.Interfaces
{
    public interface IItemService
    {
        Task<(List<Item> Items, int TotalCount)> GetAllItemsPaginatedAsync(int page, int limit, string search);
        Task<Item> GetItemByIdAsync(Guid id);
        Task<Item> AddItemAsync(ItemDto itemDto, Guid performedByUserId);
        Task<Item> UpdateItemAsync(Guid Id, ItemDto itemDto, Guid performedByUserId);
        Task<Item> DeleteItemAsync(Guid id, Guid performedByUserId);
        Task<int> GetTotalCountOfItems();

        Task<List<Item>> GetAllItemNamesAndIdsAsync();
    }
}
