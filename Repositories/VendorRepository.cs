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
                var query = @"SELECT * FROM ""Vendor"";";
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
                    INSERT INTO ""vendors"" (""Id"", ""Name"", ""Email"")
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

                string selectQuery = @"SELECT * FROM ""vendors"" WHERE ""Id"" = @Id;";
                var existingVendor = await dbConnection.QueryFirstOrDefaultAsync<Vendor>(selectQuery, new { vendor.Id });

                if (existingVendor == null)
                {
                    throw new KeyNotFoundException($"Vendor with ID {vendor.Id} not found.");
                }

                string deleteQuery = @"DELETE FROM ""vendors"" WHERE ""Id"" = @Id;";
                await dbConnection.ExecuteAsync(deleteQuery, new { vendor.Id });

                return existingVendor;
            }
        }

        public async Task<Vendor> GetVendorById(Guid vendorId)
        {
            using (var dbConnection = _dapperDbContext.CreateConnection())
            {
                dbConnection.Open();

                string query = @"SELECT * FROM ""Vendor"" WHERE ""Id"" = @Id;";
                var vendor = await dbConnection.QueryFirstOrDefaultAsync<Vendor>(query, new { Id = vendorId });
                return vendor;
            }
        }
        public async Task<bool> Exists(Guid vendorId)  // Added missing Exists method
        {
            using (var dbConnection = _dapperDbContext.CreateConnection())
            {
                var query = @"SELECT COUNT(1) FROM ""Vendor"" WHERE ""Id"" = @Id;";
                var exists = await dbConnection.ExecuteScalarAsync<bool>(query, new { Id = vendorId });
                return exists;
            }
        }

        public async Task<(List<Vendor> Vendors, int TotalCount)> SearchVendors(string searchTerm, int pageNumber, int pageSize)
        {
            using (var dbConnection = _dapperDbContext.CreateConnection())
            {
                dbConnection.Open();

                // Basic search filter (case-insensitive)
                var searchFilter = "%" + searchTerm.ToLower() + "%";

                // Query to get total count matching search
                var countQuery = @"
            SELECT COUNT(*) FROM ""Vendor""
            WHERE LOWER(""Name"") LIKE @SearchFilter
               OR LOWER(""Email"") LIKE @SearchFilter;
        ";

                // Query to get paginated results matching search
                var dataQuery = @"
            SELECT * FROM ""Vendor""
            WHERE LOWER(""Name"") LIKE @SearchFilter
               OR LOWER(""Email"") LIKE @SearchFilter
            ORDER BY ""Name""
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        ";

                int offset = (pageNumber - 1) * pageSize;

                // Get total count for pagination
                var totalCount = await dbConnection.ExecuteScalarAsync<int>(countQuery, new { SearchFilter = searchFilter });

                // Get paginated data
                var vendors = await dbConnection.QueryAsync<Vendor>(dataQuery, new { SearchFilter = searchFilter, Offset = offset, PageSize = pageSize });

                return (vendors.ToList(), totalCount);
            }
        }

    }
}
