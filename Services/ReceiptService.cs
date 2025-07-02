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
                ReceiptDetails = new List<ReceiptDetail>()
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
                   var stock = await  _stockService.GetStockByItemIdAsync(detailDto.ItemId);
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
                    else{
                        stock.CurrentQuantity += detailDto.Quantity;
                        await _stockService.UpdateStockAsync(stock);
                    }
           

                }
            }
            catch (Exception ex) { 
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
    }

  
}
