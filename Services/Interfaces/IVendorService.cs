using Inventory_Mgmt_System.Models;

namespace Inventory_Mgmt_System.Services.Interfaces
{
    public interface IVendorService
    {
        Task<List<Vendor>> GetAllVendorsAsync();
        Task<Vendor> GetVendorByIdAsync(Guid vendorId);
        Task<Vendor> AddVendorAsync(Vendor vendor, Guid performedByUserId);
        Task<Vendor> DeleteVendorAsync(Guid vendorId, Guid performedByUserId);
    }
}
