using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;

namespace Inventory_Mgmt_System.Services.Interfaces
{
    public interface IReceiptService
    {
        Task<Receipt> CreateReceiptAsync(ReceiptCreateDto receiptDto);
        Task<Receipt> GetReceiptByIdAsync(Guid id);
        Task<IEnumerable<Receipt>> GetAllReceiptsAsync();
        Task<bool> DeleteReceiptAsync(Guid id);

        Task<Receipt> UpdateReceiptAsync(Guid id, ReceiptUpdateDto receiptDto);

        Task<IEnumerable<ReceiptDetail>> GetAllReceiptDetailsAsync();
        Task<IEnumerable<ReceiptDetail>> GetSimplifiedReceiptDetailsAsync();

        Task<bool> IsItemUsedInAnyReceiptAsync(Guid id);

        Task<List<TopItemDto>> GetTop10Item();
        Task<(List<Receipt> Receipts, int TotalCount)> GetAllReceiptsPaginatedAsync(int page, int limit);
        Task<List<DailyTotalPriceDto>> GetDailyTotalReceiptValueAsync();
    }
}
