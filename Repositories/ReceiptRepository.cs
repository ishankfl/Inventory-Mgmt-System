using Inventory_Mgmt_System.Data;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Mgmt_System.Repositories
{
    public class ReceiptRepository : IReceiptRepository
    {
        private readonly AppDbContext _context;

        public ReceiptRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Receipt> CreateReceiptAsync(Receipt receipt)
        {
            await _context.Receipts.AddAsync(receipt);
            await _context.SaveChangesAsync();
            return receipt;
        }

        public async Task<Receipt> GetReceiptByIdAsync(Guid id)
        {
            return await _context.Receipts
                .Include(r => r.Vendor)
                .Include(r => r.ReceiptDetails)
                .ThenInclude(rd => rd.Item)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Receipt>> GetAllReceiptsAsync()
        {
            return await _context.Receipts
                .Include(r => r.Vendor)
                .Include(r => r.ReceiptDetails)
                .ThenInclude(rd => rd.Item)
                .ToListAsync();
        }

        public async Task<bool> DeleteReceiptAsync(Guid id)
        {
            var receipt = await _context.Receipts.FindAsync(id);
            if (receipt == null) return false;

            _context.Receipts.Remove(receipt);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
