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

        public ReceiptService(
            IReceiptRepository receiptRepository,
            IVendorRepository vendorRepository,
            IItemRepository itemRepository)
        {
            _receiptRepository = receiptRepository;
            _vendorRepository = vendorRepository;
            _itemRepository = itemRepository;
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

    public class ReceiptCreateDto
    {
        public DateTime ReceiptDate { get; set; }
        public string BillNo { get; set; }
        public Guid VendorId { get; set; }
        public List<ReceiptDetailDto> ReceiptDetails { get; set; }
    }

    public class ReceiptDetailDto
    {
        public Guid ItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
    }
}
