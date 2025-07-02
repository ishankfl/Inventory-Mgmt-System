using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Inventory_Mgmt_System.Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepository;
        private readonly ILogger<StockService> _logger;

        public StockService(IStockRepository stockRepository, ILogger<StockService> logger)
        {
            _stockRepository = stockRepository;
            _logger = logger;
        }

        public async Task<List<Stock>> GetAllStocksAsync()
        {
            try
            {
                return await _stockRepository.GetAllStocksAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all stocks");
                throw;
            }
        }

        public async Task<Stock> GetStockByIdAsync(Guid id)
        {
            try
            {
                return await _stockRepository.GetStockByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting stock by ID: {id}");
                throw;
            }
        }

        public async Task<Stock> GetStockByItemIdAsync(Guid itemId)
        {
            try
            {
                return await _stockRepository.GetStockByItemIdAsync(itemId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting stock by Item ID: {itemId}");
                throw;
            }
        }

        public async Task<Stock> AddStockAsync(Stock stock)
        {
            try
            {
                // Check if stock already exists for this item
                var existingStock = await _stockRepository.GetStockByItemIdAsync(stock.ItemId);
                if (existingStock != null)
                {
                    throw new InvalidOperationException($"Stock already exists for item with ID: {stock.ItemId}");
                }

                return await _stockRepository.AddStockAsync(stock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding stock for item ID: {stock.ItemId}");
                throw;
            }
        }

        public async Task<Stock> UpdateStockAsync(Stock stock)
        {
            try
            {
                var existingStock = await _stockRepository.GetStockByIdAsync(stock.Id);
                if (existingStock == null)
                {
                    throw new KeyNotFoundException($"Stock with ID {stock.Id} not found");
                }

                return await _stockRepository.UpdateStockAsync(stock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating stock with ID: {stock.Id}");
                throw;
            }
        }

        public async Task<bool> DeleteStockAsync(Guid id)
        {
            try
            {
                return await _stockRepository.DeleteStockAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting stock with ID: {id}");
                throw;
            }
        }

        public async Task<decimal> GetCurrentQuantityByItemIdAsync(Guid itemId)
        {
            try
            {
                return await _stockRepository.GetCurrentQuantityByItemIdAsync(itemId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting current quantity for item ID: {itemId}");
                throw;
            }
        }

        public async Task<Stock> UpdateStockQuantityAsync(Guid itemId, decimal quantityChange)
        {
            try
            {
                var stock = await _stockRepository.GetStockByItemIdAsync(itemId);
                if (stock == null)
                {
                    throw new KeyNotFoundException($"Stock not found for item ID: {itemId}");
                }

                stock.CurrentQuantity += quantityChange;
                if (stock.CurrentQuantity < 0)
                {
                    throw new InvalidOperationException("Stock quantity cannot be negative");
                }

                return await _stockRepository.UpdateStockAsync(stock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating stock quantity for item ID: {itemId}");
                throw;
            }
        }
    }
}