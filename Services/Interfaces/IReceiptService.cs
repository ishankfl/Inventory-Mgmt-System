using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;

namespace Inventory_Mgmt_System.Services.Interfaces
{
    public interface IReceiptService
    {
        Task<Receipt> CreateReceiptAsync(ReceiptCreateDto receiptDto, Guid performedByUserId);
        Task<Receipt> UpdateReceiptAsync(Guid id, ReceiptUpdateDto receiptDto, Guid performedByUserId);
        Task<bool> DeleteReceiptAsync(Guid id, Guid performedByUserId);
        Task<Receipt> GetReceiptByIdAsync(Guid id);
        Task<IEnumerable<Receipt>> GetAllReceiptsAsync();
        Task<IEnumerable<ReceiptDetail>> GetAllReceiptDetailsAsync();
        Task<(List<Receipt> Receipts, int TotalCount)> GetAllReceiptsPaginatedAsync(int page, int limit);
        Task<IEnumerable<ReceiptDetail>> GetSimplifiedReceiptDetailsAsync();
        Task<bool> IsItemUsedInAnyReceiptAsync(Guid id);
        Task<List<TopItemDto>> GetTop10Item();
        Task<List<DailyTotalPriceDto>> GetDailyTotalReceiptValueAsync();
    }
}
