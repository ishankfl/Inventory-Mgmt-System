﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Inventory_Mgmt_System.Services.Interfaces;

namespace Inventory_Mgmt_System.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;

        public ItemService(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<List<Item>> GetAllItemsAsync()
        {
            return await _itemRepository.GetAllAsync();
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

        public async Task<Item> AddItemAsync(Item item)
        {
            ValidateItem(item);

            return await _itemRepository.AddAsync(item);
        }

        public async Task<Item> UpdateItemAsync(Item item)
        {
            if (item.Id == Guid.Empty)
                throw new ArgumentException("Item ID cannot be empty for update.");

            ValidateItem(item);

            var existingItem = await _itemRepository.GetByIdAsync(item.Id);
            if (existingItem == null)
                throw new KeyNotFoundException($"Item with ID {item.Id} not found.");

            return await _itemRepository.UpdateAsync(item);
        }

        public async Task<Item> DeleteItemAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Item ID cannot be empty.");

            return await _itemRepository.DeleteAsync(id);
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
    }
}
