using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Inventory_Mgmt_System.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_Mgmt_System.Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly IReceiptRepository _receiptRepository;
        private readonly IVendorRepository _vendorRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IStockService _stockService;
        private readonly IActivityServices _activityServices;

        public ReceiptService(
            IReceiptRepository receiptRepository,
            IVendorRepository vendorRepository,
            IItemRepository itemRepository,
            IStockService stockService,
            IActivityServices activityServices)
        {
            _receiptRepository = receiptRepository;
            _vendorRepository = vendorRepository;
            _itemRepository = itemRepository;
            _stockService = stockService;
            _activityServices = activityServices;
        }

        public async Task<Receipt> CreateReceiptAsync(ReceiptCreateDto receiptDto, Guid performedByUserId)
        {
            try
            {
                // Validate vendor exists
                var vendor = await _vendorRepository.GetVendorById(receiptDto.VendorId);
                if (vendor == null)
                {
                    throw new ArgumentException("Vendor not found");
                }

                // Create receipt
                var receipt = new Receipt
                {
                    Id = Guid.NewGuid(),
                    ReceiptDate = receiptDto.ReceiptDate,
                    BillNo = receiptDto.BillNo,
                    VendorId = receiptDto.VendorId,
                    ReceiptDetails = new List<ReceiptDetail>(),
                    ReceiptId = receiptDto.ReceiptId
                };

                // Add receipt details and update stock
                foreach (var detailDto in receiptDto.ReceiptDetails)
                {
                    var item = await _itemRepository.GetByIdAsync(detailDto.ItemId);
                    if (item == null)
                    {
                        throw new ArgumentException($"Item with ID {detailDto.ItemId} not found");
                    }

                    receipt.ReceiptDetails.Add(new ReceiptDetail
                    {
                        Id = Guid.NewGuid(),
                        ItemId = detailDto.ItemId,
                        Quantity = detailDto.Quantity,
                        Rate = detailDto.Rate
                    });

                    await UpdateStockQuantity(detailDto.ItemId, detailDto.Quantity);
                }

                var createdReceipt = await _receiptRepository.CreateReceiptAsync(receipt);

                if (createdReceipt != null)
                {
                    await LogActivity(
                        $"Receipt created (Bill No: {receipt.BillNo}) with {receipt.ReceiptDetails.Count} items",
                        "success",
                        ActivityType.ReceiptCreated,
                        performedByUserId);
                }

                return createdReceipt;
            }
            catch (Exception ex)
            {
                await LogActivity(
                    $"Failed to create receipt: {ex.Message}",
                    "danger",
                    ActivityType.ReceiptCreationFailed,
                    performedByUserId);
                throw;
            }
        }

        public async Task<Receipt> GetReceiptByIdAsync(Guid id)
        {
            return await _receiptRepository.GetReceiptByIdAsync(id);
        }

        public async Task<IEnumerable<Receipt>> GetAllReceiptsAsync()
        {
            return await _receiptRepository.GetAllReceiptsAsync();
        }

        public async Task<bool> DeleteReceiptAsync(Guid id, Guid performedByUserId)
        {
            var receipt = await _receiptRepository.GetReceiptByIdAsync(id);
            if (receipt == null)
            {
                return false;
            }

            // First update stocks for all items in the receipt
            foreach (var detail in receipt.ReceiptDetails)
            {
                await UpdateStockQuantity(detail.ItemId, -detail.Quantity);
            }

            var result = await _receiptRepository.DeleteReceiptAsync(id);

            if (result)
            {
                await LogActivity(
                    $"Receipt deleted (Bill No: {receipt.BillNo})",
                    "danger",
                    ActivityType.ReceiptDeleted,
                    performedByUserId);
            }

            return result;
        }

        public async Task<Receipt> UpdateReceiptAsync(Guid id, ReceiptUpdateDto receiptDto, Guid performedByUserId)
        {
            try
            {
                var existingReceipt = await _receiptRepository.GetReceiptByIdAsync(id);
                if (existingReceipt == null)
                {
                    throw new KeyNotFoundException($"Receipt with ID {id} not found");
                }

                var vendorExists = await _vendorRepository.Exists(receiptDto.VendorId);
                if (!vendorExists)
                {
                    throw new ArgumentException("Vendor not found");
                }

                string originalBillNo = existingReceipt.BillNo;
                existingReceipt.ReceiptDate = receiptDto.ReceiptDate;
                existingReceipt.BillNo = receiptDto.BillNo;
                existingReceipt.VendorId = receiptDto.VendorId;

                var stockChanges = CalculateStockChanges(existingReceipt, receiptDto);

                existingReceipt.ReceiptDetails = UpdateReceiptDetails(existingReceipt, receiptDto);

                var updatedReceipt = await _receiptRepository.UpdateReceiptAsync(existingReceipt);

                await ApplyStockChanges(stockChanges);

                if (updatedReceipt != null)
                {
                    await LogActivity(
                        $"Receipt updated (Bill No: {originalBillNo} -> {updatedReceipt.BillNo})",
                        "info",
                        ActivityType.ReceiptUpdated,
                        performedByUserId);
                }

                return updatedReceipt;
            }
            catch (Exception ex)
            {
                await LogActivity(
                    $"Failed to update receipt: {ex.Message}",
                    "danger",
                    ActivityType.ReceiptUpdateFailed,
                    performedByUserId);
                throw;
            }
        }

        public async Task<IEnumerable<ReceiptDetail>> GetAllReceiptDetailsAsync()
        {
            return await _receiptRepository.GetAllReceiptDetailsAsync();
        }

        public async Task<(List<Receipt> Receipts, int TotalCount)> GetAllReceiptsPaginatedAsync(int page, int limit)
        {
            return await _receiptRepository.GetAllReceiptsPaginatedAsync(page, limit);
        }

        public async Task<IEnumerable<ReceiptDetail>> GetSimplifiedReceiptDetailsAsync()
        {
            return await _receiptRepository.GetSimplifiedReceiptDetailsAsync();
        }

        public async Task<bool> IsItemUsedInAnyReceiptAsync(Guid id)
        {
            return await _receiptRepository.IsItemUsedInAnyReceiptAsync(id);
        }

        public async Task<List<TopItemDto>> GetTop10Item()
        {
            return await _receiptRepository.GetTop10Item();
        }

        public async Task<List<DailyTotalPriceDto>> GetDailyTotalReceiptValueAsync()
        {
            return await _receiptRepository.GetDailyTotalReceiptValueAsync();
        }

        #region Private Methods

        private async Task UpdateStockQuantity(Guid itemId, decimal quantityChange)
        {
            var stock = await _stockService.GetStockByItemIdAsync(itemId);
            if (stock == null)
            {
                if (quantityChange > 0)
                {
                    await _stockService.AddStockAsync(new Stock
                    {
                        CurrentQuantity = quantityChange,
                        ItemId = itemId
                    });
                }
            }
            else
            {
                stock.CurrentQuantity += quantityChange;
                await _stockService.UpdateStockAsync(stock);
            }
        }

        private Dictionary<Guid, decimal> CalculateStockChanges(Receipt existingReceipt, ReceiptUpdateDto receiptDto)
        {
            var stockChanges = new Dictionary<Guid, decimal>();

            // Calculate changes for existing details
            foreach (var detailDto in receiptDto.ReceiptDetails)
            {
                var existingDetail = existingReceipt.ReceiptDetails?
                    .FirstOrDefault(rd => rd.Id == detailDto.Id || (detailDto.Id == null && rd.ItemId == detailDto.ItemId));

                if (existingDetail != null)
                {
                    var quantityDifference = detailDto.Quantity - existingDetail.Quantity;
                    if (quantityDifference != 0)
                    {
                        UpdateStockChange(stockChanges, detailDto.ItemId, quantityDifference);
                    }
                }
                else
                {
                    UpdateStockChange(stockChanges, detailDto.ItemId, detailDto.Quantity);
                }
            }

            // Calculate changes for removed details
            var removedDetails = existingReceipt.ReceiptDetails?
                .Where(ed => !receiptDto.ReceiptDetails.Any(rd => rd.Id == ed.Id))
                .ToList() ?? new List<ReceiptDetail>();

            foreach (var removedDetail in removedDetails)
            {
                UpdateStockChange(stockChanges, removedDetail.ItemId, -removedDetail.Quantity);
            }

            return stockChanges;
        }

        private void UpdateStockChange(Dictionary<Guid, decimal> stockChanges, Guid itemId, decimal quantityChange)
        {
            if (stockChanges.ContainsKey(itemId))
            {
                stockChanges[itemId] += quantityChange;
            }
            else
            {
                stockChanges.Add(itemId, quantityChange);
            }
        }

        private List<ReceiptDetail> UpdateReceiptDetails(Receipt existingReceipt, ReceiptUpdateDto receiptDto)
        {
            var updatedDetails = new List<ReceiptDetail>();

            foreach (var detailDto in receiptDto.ReceiptDetails)
            {
                var existingDetail = existingReceipt.ReceiptDetails?
                    .FirstOrDefault(rd => rd.Id == detailDto.Id || (detailDto.Id == null && rd.ItemId == detailDto.ItemId));

                if (existingDetail != null)
                {
                    existingDetail.ItemId = detailDto.ItemId;
                    existingDetail.Quantity = detailDto.Quantity;
                    existingDetail.Rate = detailDto.Rate;
                    updatedDetails.Add(existingDetail);
                }
                else
                {
                    updatedDetails.Add(new ReceiptDetail
                    {
                        Id = Guid.NewGuid(),
                        ReceiptId = existingReceipt.Id,
                        ItemId = detailDto.ItemId,
                        Quantity = detailDto.Quantity,
                        Rate = detailDto.Rate
                    });
                }
            }

            return updatedDetails;
        }

        private async Task ApplyStockChanges(Dictionary<Guid, decimal> stockChanges)
        {
            foreach (var stockChange in stockChanges)
            {
                await UpdateStockQuantity(stockChange.Key, stockChange.Value);
            }
        }

        private async Task LogActivity(string action, string status, ActivityType type, Guid userId)
        {
            var activity = new ActivityDTO
            {
                Action = action,
                Status = status,
                Type = type,
                UserId = userId
            };
            await _activityServices.AddNewActivity(activity);
        }

        #endregion
    }
}