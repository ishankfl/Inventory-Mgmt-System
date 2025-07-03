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
    }

}
