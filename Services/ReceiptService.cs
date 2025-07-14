using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Inventory_Mgmt_System.Services.Interfaces;

namespace Inventory_Mgmt_System.Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly IReceiptRepository _receiptRepository;
        private readonly IVendorRepository _vendorRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IStockService _stockService;

        public ReceiptService(
            IReceiptRepository receiptRepository,
            IVendorRepository vendorRepository,
            IItemRepository itemRepository, IStockService stockService)
        {
            _receiptRepository = receiptRepository;
            _vendorRepository = vendorRepository;
            _itemRepository = itemRepository;
            _stockService = stockService;
        }

        public async Task<Receipt> CreateReceiptAsync(ReceiptCreateDto receiptDto)
        {
            // Validate vendor exists
            var vendor = await _vendorRepository.GetVendorById(receiptDto.VendorId);
            if (vendor == null)
                throw new ArgumentException("Vendor not found");

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
            try
            {
                // Add receipt details
                foreach (var detailDto in receiptDto.ReceiptDetails)
                {
                    var item = await _itemRepository.GetByIdAsync(detailDto.ItemId);
                    if (item == null)
                        throw new ArgumentException($"Item with ID {detailDto.ItemId} not found");

                    receipt.ReceiptDetails.Add(new ReceiptDetail
                    {
                        Id = Guid.NewGuid(),
                        ItemId = detailDto.ItemId,
                        Quantity = detailDto.Quantity,
                        Rate = detailDto.Rate
                    });
                    var stock = await _stockService.GetStockByItemIdAsync(detailDto.ItemId);
                    if (stock == null)
                    {
                        await _stockService.AddStockAsync(
                              new Stock
                              {
                                  CurrentQuantity = detailDto.Quantity,
                                  ItemId = detailDto.ItemId,
                              }
                              );
                    }
                    else
                    {
                        stock.CurrentQuantity += detailDto.Quantity;
                        await _stockService.UpdateStockAsync(stock);
                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }



            return await _receiptRepository.CreateReceiptAsync(receipt);
        }

        public async Task<Receipt> GetReceiptByIdAsync(Guid id)
        {
            return await _receiptRepository.GetReceiptByIdAsync(id);
        }

        public async Task<IEnumerable<Receipt>> GetAllReceiptsAsync()
        {
            return await _receiptRepository.GetAllReceiptsAsync();
        }

        public async Task<bool> DeleteReceiptAsync(Guid id)
        {
            return await _receiptRepository.DeleteReceiptAsync(id);
        }

        public async Task<Receipt> UpdateReceiptAsync(Guid id, ReceiptUpdateDto receiptDto)
        {
            var existingReceipt = await _receiptRepository.GetReceiptByIdAsync(id);
            if (existingReceipt == null) 
                throw new KeyNotFoundException($"Receipt with ID {id} not found");

            var vendorExists = await _vendorRepository.Exists(receiptDto.VendorId); 
            if (!vendorExists)
                throw new ArgumentException("Vendor not found");

            existingReceipt.ReceiptDate = receiptDto.ReceiptDate;
            existingReceipt.BillNo = receiptDto.BillNo;
            existingReceipt.VendorId = receiptDto.VendorId;

            var stockChanges = new Dictionary<Guid, decimal>();

            var updatedDetails = new List<ReceiptDetail>();
            foreach (var detailDto in receiptDto.ReceiptDetails ) 
            {
                var itemExists = await _itemRepository.Exists(detailDto.ItemId);
                if (!itemExists)
                    throw new ArgumentException($"Item with ID {detailDto.ItemId} not found");

                var existingDetail = existingReceipt.ReceiptDetails?
                    .FirstOrDefault(rd => rd.Id == detailDto.Id || (detailDto.Id == null && rd.ItemId == detailDto.ItemId));

                if (existingDetail != null)
                {
                    var quantityDifference = detailDto.Quantity - existingDetail.Quantity;

                    if (quantityDifference != 0)
                    {
                        if (stockChanges.ContainsKey(detailDto.ItemId))
                            stockChanges[detailDto.ItemId] += quantityDifference;
                        else
                            stockChanges.Add(detailDto.ItemId, quantityDifference);
                    }

                    existingDetail.ItemId = detailDto.ItemId;
                    existingDetail.Quantity = detailDto.Quantity;
                    existingDetail.Rate = detailDto.Rate;
                    updatedDetails.Add(existingDetail);
                }
                else
                {
                    var newDetail = new ReceiptDetail
                    {
                        Id = Guid.NewGuid(),
                        ReceiptId = id,
                        ItemId = detailDto.ItemId,
                        Quantity = detailDto.Quantity,
                        Rate = detailDto.Rate
                    };
                    updatedDetails.Add(newDetail);

                    if (stockChanges.ContainsKey(detailDto.ItemId))
                        stockChanges[detailDto.ItemId] += detailDto.Quantity;
                    else
                        stockChanges.Add(detailDto.ItemId, detailDto.Quantity);
                }
            }

            var removedDetails = existingReceipt.ReceiptDetails?
                .Where(ed => !receiptDto.ReceiptDetails.Any(rd => rd.Id == ed.Id)) // Ensure case matches
                .ToList() ?? new List<ReceiptDetail>();

            foreach (var removedDetail in removedDetails)
            {
                if (stockChanges.ContainsKey(removedDetail.ItemId))
                    stockChanges[removedDetail.ItemId] -= removedDetail.Quantity;
                else
                    stockChanges.Add(removedDetail.ItemId, -removedDetail.Quantity);
            }

            existingReceipt.ReceiptDetails = updatedDetails;

            var updatedReceipt = await _receiptRepository.UpdateReceiptAsync(existingReceipt);

            foreach (var stockChange in stockChanges)
            {
                var stock = await _stockService.GetStockByItemIdAsync(stockChange.Key);
                if (stock == null)
                {
                    if (stockChange.Value > 0)
                    {
                        await _stockService.AddStockAsync(new Stock
                        {
                            CurrentQuantity = stockChange.Value,
                            ItemId = stockChange.Key
                        });
                    }
                }
                else
                {
                    stock.CurrentQuantity += stockChange.Value;
                    await _stockService.UpdateStockAsync(stock);
                }
            }

            return updatedReceipt;
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

       public async  Task<List<DailyTotalPriceDto>> GetDailyTotalReceiptValueAsync()
        {
            return await _receiptRepository.GetDailyTotalReceiptValueAsync();
        }

    } 

    }
