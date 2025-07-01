using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Inventory_Mgmt_System.Services.Interfaces;
using System.Text.RegularExpressions;

namespace Inventory_Mgmt_System.Services
{
    public class VendorService : IVendorService
    {
        private readonly IVendorRepository _vendorRepository;

        public VendorService(IVendorRepository vendorRepository)
        {
            _vendorRepository = vendorRepository;
        }

        public async Task<List<Vendor>> GetAllVendorsAsync()
        {
            return await _vendorRepository.GetAllVendors();
        }

        public async Task<Vendor> AddVendorAsync(Vendor vendor)
        {
            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor), "Vendor object is required.");

            if (string.IsNullOrWhiteSpace(vendor.Name))
                throw new ArgumentException("Vendor name is required.", nameof(vendor.Name));

            if (string.IsNullOrWhiteSpace(vendor.Email))
                throw new ArgumentException("Vendor email is required.", nameof(vendor.Email));

            if (!IsValidEmail(vendor.Email))
                throw new ArgumentException("Invalid email format.", nameof(vendor.Email));

            var existingVendors = await _vendorRepository.GetAllVendors();
            if (existingVendors.Any(v => v.Email.ToLower() == vendor.Email.ToLower()))
                throw new InvalidOperationException("A vendor with the same email already exists.");

            return await _vendorRepository.AddNewVendor(vendor);
        }

        public async Task<Vendor> DeleteVendorAsync(Guid vendorId)
        {
            if (vendorId == Guid.Empty)
                throw new ArgumentException("Vendor ID cannot be empty.", nameof(vendorId));

            var vendor = await _vendorRepository.GetVendorById(vendorId);
            if (vendor == null)
                throw new KeyNotFoundException($"Vendor with ID {vendorId} not found.");

            return await _vendorRepository.DeleteVendor(vendor);
        }

        public async Task<Vendor> GetVendorByIdAsync(Guid vendorId)
        {
            if (vendorId == Guid.Empty)
                throw new ArgumentException("Vendor ID cannot be empty.", nameof(vendorId));

            var vendor = await _vendorRepository.GetVendorById(vendorId);
            if (vendor == null)
                throw new KeyNotFoundException($"Vendor with ID {vendorId} not found.");

            return vendor;
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase);
        }
    }
}
