using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Inventory_Mgmt_System.Services.Interfaces;

namespace Inventory_Mgmt_System.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IReceiptService _receiptService;
        private readonly IActivityServices _activityServices;

        public ItemService(
            IItemRepository itemRepository,
            IReceiptService receiptService,
            IActivityServices activityServices)
        {
            _itemRepository = itemRepository;
            _receiptService = receiptService;
            _activityServices = activityServices;
        }

        public async Task<List<Item>> GetAllItemNamesAndIdsAsync()
        {
            return await _itemRepository.GetAllItemNamesAndIdsAsync();
         }
        public async Task<(List<Item> Items, int TotalCount)> GetAllItemsPaginatedAsync(int page, int limit, string search)
        {
            return await _itemRepository.GetAllPaginatedAsync(page, limit, search);
        }
        public async Task<Item> GetItemByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Item ID cannot be empty.");

            var item = await _itemRepository.GetByIdAsync(id);
            if (item == null)
                throw new KeyNotFoundException($"Item with ID {id} not found.");

            return item;
        }

        public async Task<Item> AddItemAsync(ItemDto itemDto, Guid performedByUserId)
        {
            Item item = new Item
            {
                Name = itemDto.Name,
                Price = itemDto.Price,
                Unit = itemDto.Unit
            };
            ValidateItem(item);

            var addedItem = await _itemRepository.AddAsync(item);

            if (addedItem != null)
            {
                var activity = new ActivityDTO
                {
                    Action = $"New item added: {item.Name}",
                    Status = "success",
                    Type = ActivityType.ItemAdded,
                    UserId = performedByUserId
                };
                await _activityServices.AddNewActivity(activity);
            }

            return addedItem;
        }

        public async Task<Item> UpdateItemAsync(Guid Id, ItemDto itemDto, Guid performedByUserId)
        {
            if (Id == Guid.Empty)
                throw new ArgumentException("Item ID cannot be empty for update.");

            Item item = new Item
            {
                Id = Id,
                Price = itemDto.Price,
                Unit = itemDto.Unit,
                Name = itemDto.Name
            };
            ValidateItem(item);

            var existingItem = await _itemRepository.GetByIdAsync(item.Id);
            if (existingItem == null)
                throw new KeyNotFoundException($"Item with ID {item.Id} not found.");

            var updatedItem = await _itemRepository.UpdateAsync(item);

            if (updatedItem != null)
            {
                var activity = new ActivityDTO
                {
                    Action = $"Item updated: {item.Name}",
                    Status = "info",
                    Type = ActivityType.ItemUpdated,
                    UserId = performedByUserId
                };
                await _activityServices.AddNewActivity(activity);
            }

            return updatedItem;
        }

        public async Task<Item> DeleteItemAsync(Guid id, Guid performedByUserId)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Item ID cannot be empty.");

            var isUsed = await _receiptService.IsItemUsedInAnyReceiptAsync(id);
            if (isUsed)
                throw new InvalidOperationException("Cannot delete this item because it is used in one or more receipts.");

            var itemToDelete = await _itemRepository.GetByIdAsync(id);
            if (itemToDelete == null)
                throw new KeyNotFoundException($"Item with ID {id} not found.");

            var deletedItem = await _itemRepository.DeleteAsync(id);

            if (deletedItem != null)
            {
                var activity = new ActivityDTO
                {
                    Action = $"Item deleted: {itemToDelete.Name}",
                    Status = "danger",
                    Type = ActivityType.ItemDeleted,
                    UserId = performedByUserId
                };
                await _activityServices.AddNewActivity(activity);
            }

            return deletedItem;
        }

        private void ValidateItem(Item item)
        {
            if (item == null)
                throw new ArgumentException("Item cannot be null.");

            if (string.IsNullOrWhiteSpace(item.Name))
                throw new ArgumentException("Item Name is required.");

            if (item.Name.Length > 255)
                throw new ArgumentException("Item Name cannot exceed 255 characters.");

            if (string.IsNullOrWhiteSpace(item.Unit))
                throw new ArgumentException("Item Unit is required.");

            if (item.Unit.Length > 50)
                throw new ArgumentException("Item Unit cannot exceed 50 characters.");
        }

        public async Task<int> GetTotalCountOfItems()
        {
            return await _itemRepository.GetTotalCountOfItems();
        }
    }
}