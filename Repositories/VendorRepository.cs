using Inventory_Mgmt_System.Data;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Dapper;
using System.Data;

namespace Inventory_Mgmt_System.Repositories
{
    public class VendorRepository : IVendorRepository
    {
        private readonly DapperDbContext _dapperDbContext;

        public VendorRepository(DapperDbContext dapperDbContext)
        {
            _dapperDbContext = dapperDbContext;
        }

        public async Task<List<Vendor>> GetAllVendors()
        {
            using (var dbConnection = _dapperDbContext.CreateConnection())
            {
                dbConnection.Open();
                var query = @"SELECT * FROM ""Vendors"";";
                var vendors = await dbConnection.QueryAsync<Vendor>(query);
                return vendors.ToList();
            }
        }

        public async Task<Vendor> AddNewVendor(Vendor vendor)
        {
            using (var dbConnection = _dapperDbContext.CreateConnection())
            {
                dbConnection.Open();

                string query = @"
                    INSERT INTO ""Vendors"" (""Id"", ""Name"", ""Email"")
                    VALUES (@Id, @Name, @Email)
                    RETURNING ""Id"";";

                // Ensure Id is set (if not already)
                if (vendor.Id == Guid.Empty)
                {
                    vendor.Id = Guid.NewGuid();
                }

                var vendorId = await dbConnection.ExecuteScalarAsync<Guid>(query, vendor);
                return vendor;
            }
        }

        public async Task<Vendor> DeleteVendor(Vendor vendor)
        {
            using (var dbConnection = _dapperDbContext.CreateConnection())
            {
                dbConnection.Open();

                string selectQuery = @"SELECT * FROM ""Vendors"" WHERE ""Id"" = @Id;";
                var existingVendor = await dbConnection.QueryFirstOrDefaultAsync<Vendor>(selectQuery, new { vendor.Id });

                if (existingVendor == null)
                {
                    throw new KeyNotFoundException($"Vendor with ID {vendor.Id} not found.");
                }

                string deleteQuery = @"DELETE FROM ""Vendors"" WHERE ""Id"" = @Id;";
                await dbConnection.ExecuteAsync(deleteQuery, new { vendor.Id });

                return existingVendor;
            }
        }

        public async Task<Vendor> GetVendorById(Guid vendorId)
        {
            using (var dbConnection = _dapperDbContext.CreateConnection())
            {
                dbConnection.Open();

                string query = @"SELECT * FROM ""Vendors"" WHERE ""Id"" = @Id;";
                var vendor = await dbConnection.QueryFirstOrDefaultAsync<Vendor>(query, new { Id = vendorId });
                return vendor;
            }
        }
    }
}
