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
                    INSERT INTO ""Vendor"" (""Id"", ""Name"", ""Email"")
                    VALUES (@Id, @Name, @Email)
                    RETURNING ""Id"";";

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

                string selectQuery = @"SELECT * FROM ""Vendor"" WHERE ""Id"" = @Id;";
                var existingVendor = await dbConnection.QueryFirstOrDefaultAsync<Vendor>(selectQuery, new { vendor.Id });

                if (existingVendor == null)
                {
                    throw new KeyNotFoundException($"Vendor with ID {vendor.Id} not found.");
                }

                string deleteQuery = @"DELETE FROM ""Vendor"" WHERE ""Id"" = @Id;";
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

        public async Task<bool> Exists(Guid vendorId)
        {
            using (var dbConnection = _dapperDbContext.CreateConnection())
            {
                dbConnection.Open();

                var query = @"SELECT COUNT(1) FROM ""Vendor"" WHERE ""Id"" = @Id;";
                var count = await dbConnection.ExecuteScalarAsync<int>(query, new { Id = vendorId });

                return count > 0;
            }
        }

        public async Task<(List<Vendor> Vendors, int TotalCount)> SearchVendors(string searchTerm, int pageNumber, int pageSize)
        {
            using (var dbConnection = _dapperDbContext.CreateConnection())
            {
                dbConnection.Open();

                var searchFilter = "%" + searchTerm.ToLower() + "%";

                var countQuery = @"
                    SELECT COUNT(*) FROM ""Vendor""
                    WHERE LOWER(""Name"") LIKE @SearchFilter
                       OR LOWER(""Email"") LIKE @SearchFilter;
                ";

                var dataQuery = @"
                    SELECT * FROM ""Vendor""
                    WHERE LOWER(""Name"") LIKE @SearchFilter
                       OR LOWER(""Email"") LIKE @SearchFilter
                    ORDER BY ""Name""
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
                ";

                int offset = (pageNumber - 1) * pageSize;

                var totalCount = await dbConnection.ExecuteScalarAsync<int>(countQuery, new { SearchFilter = searchFilter });
                var vendors = await dbConnection.QueryAsync<Vendor>(dataQuery, new { SearchFilter = searchFilter, Offset = offset, PageSize = pageSize });

                return (vendors.ToList(), totalCount);
            }
        }
    }
}
