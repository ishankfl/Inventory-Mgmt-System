using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;

namespace Inventory_Mgmt_System.Services.Interfaces
{
    public interface IItemService
    {
        Task<List<Item>> GetAllItemsAsync();
        Task<Item> GetItemByIdAsync(Guid id);
        Task<Item> AddItemAsync(ItemDto item);
        Task<Item> UpdateItemAsync(Item item);
        Task<Item> DeleteItemAsync(Guid id);

        Task<int> GetTotalCountOfItems();
    }
}
