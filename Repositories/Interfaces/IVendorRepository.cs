using Inventory_Mgmt_System.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory_Mgmt_System.Repositories.Interfaces
{
    public interface IVendorRepository
    {
        Task<List<Vendor>> GetAllVendors();

        Task<Vendor> AddNewVendor(Vendor vendor);

        Task<Vendor> DeleteVendor(Vendor vendor);

        Task<Vendor> GetVendorById(Guid vendorId);

        Task<bool> Exists(Guid vendorId);
    }
}
