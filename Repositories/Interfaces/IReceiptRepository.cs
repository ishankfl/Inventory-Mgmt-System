using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;

namespace Inventory_Mgmt_System.Repositories.Interfaces
{
    public interface IReceiptRepository
    {
        Task<Receipt> CreateReceiptAsync(Receipt receipt);
        Task<Receipt> GetReceiptByIdAsync(Guid id);
        Task<IEnumerable<Receipt>> GetAllReceiptsAsync();
        Task<bool> DeleteReceiptAsync(Guid id);
        Task<Receipt> UpdateReceiptAsync(Receipt receipt);

        Task<IEnumerable<ReceiptDetail>> GetAllReceiptDetailsAsync();
        Task<IEnumerable<ReceiptDetail>> GetSimplifiedReceiptDetailsAsync();
        Task<bool> IsItemUsedInAnyReceiptAsync(Guid itemId);

       // Task<List<Item>> GetTop10Item();

        Task<List<TopItemDto>> GetTop10Item();

    }

}
