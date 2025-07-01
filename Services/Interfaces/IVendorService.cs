using Inventory_Mgmt_System.Models;

namespace Inventory_Mgmt_System.Services.Interfaces
{
    public interface IVendorService
    {
        Task<List<Vendor>> GetAllVendorsAsync();
        Task<Vendor> AddVendorAsync(Vendor vendor);
        Task<Vendor> DeleteVendorAsync(Guid vendorId);
        Task<Vendor> GetVendorByIdAsync(Guid vendorId);
    }
}
