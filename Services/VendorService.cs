using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Inventory_Mgmt_System.Services.Interfaces;
using Inventory_Mgmt_System.Dtos;
using System.Text.RegularExpressions;

namespace Inventory_Mgmt_System.Services
{
    public class VendorService : IVendorService
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly IActivityServices _activityServices;

        public VendorService(IVendorRepository vendorRepository, IActivityServices activityServices)
        {
            _vendorRepository = vendorRepository;
            _activityServices = activityServices;
        }

        public async Task<List<Vendor>> GetAllVendorsAsync()
        {
            return await _vendorRepository.GetAllVendors();
        }

        public async Task<Vendor> AddVendorAsync(Vendor vendor, Guid performedByUserId)
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

            var addedVendor = await _vendorRepository.AddNewVendor(vendor);

            if (addedVendor != null)
            {
                var activity = new ActivityDTO
                {
                    Action = $"New vendor added: {vendor.Name}",
                    Status = "success",
                    Type = ActivityType.VendorAdded,
                    UserId = performedByUserId
                };
                await _activityServices.AddNewActivity(activity);
            }

            return addedVendor;
        }

        public async Task<Vendor> DeleteVendorAsync(Guid vendorId, Guid performedByUserId)
        {
            if (vendorId == Guid.Empty)
                throw new ArgumentException("Vendor ID cannot be empty.", nameof(vendorId));

            var vendor = await _vendorRepository.GetVendorById(vendorId);
            if (vendor == null)
                throw new KeyNotFoundException($"Vendor with ID {vendorId} not found.");

            var deletedVendor = await _vendorRepository.DeleteVendor(vendor);

            if (deletedVendor != null)
            {
                var activity = new ActivityDTO
                {
                    Action = $"Vendor deleted: {vendor.Name}",
                    Status = "danger",
                    Type = ActivityType.VendorDeleted,
                    UserId = performedByUserId
                };
                await _activityServices.AddNewActivity(activity);
            }

            return deletedVendor;
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
        public async Task<(List<Vendor> Vendors, int TotalCount)> SearchVendorsAsync(string searchTerm, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            return await _vendorRepository.SearchVendors(searchTerm, pageNumber, pageSize);
        }

    }
}