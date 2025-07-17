using Dapper;
using Inventory_Mgmt_System.Data;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;

namespace Inventory_Mgmt_System.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly DapperDbContext _dapperDbContext;

        public DepartmentRepository(DapperDbContext dapperDbContext)
        {
            _dapperDbContext = dapperDbContext;
        }

        public async Task<List<Department>> GetAllAsync()
        {
            using var connection = _dapperDbContext.CreateConnection();
            const string query = @"SELECT * FROM ""Departments""";
            var departments = await connection.QueryAsync<Department>(query);
            return departments.ToList();
        }
        public async Task<(List<Department> Items, int TotalCount, int TotalPages)> SearchDepartmentsAsync(string? searchTerm, int pageNumber, int pageSize)
        {
            using var connection = _dapperDbContext.CreateConnection();

            // Step 1: Get total count of matching rows
            const string countQuery = @"
        SELECT COUNT(*) FROM ""Departments""
        WHERE (@SearchTerm IS NULL OR LOWER(""Name"") LIKE LOWER(CONCAT('%', @SearchTerm, '%')))
    ";

            var totalCount = await connection.ExecuteScalarAsync<int>(countQuery, new
            {
                SearchTerm = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm
            });

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // Step 2: Get paged, filtered data
            const string dataQuery = @"
        SELECT * FROM ""Departments""
        WHERE (@SearchTerm IS NULL OR LOWER(""Name"") LIKE LOWER(CONCAT('%', @SearchTerm, '%')))
        ORDER BY ""Name""
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
    ";

            var departments = await connection.QueryAsync<Department>(dataQuery, new
            {
                SearchTerm = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm,
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize
            });

            return (departments.ToList(), totalCount, totalPages);
        }

        public async Task<Department?> GetByIdAsync(Guid id)
        {
            using var connection = _dapperDbContext.CreateConnection();
            const string query = @"SELECT * FROM ""Departments"" WHERE ""Id"" = @Id";
            return await connection.QueryFirstOrDefaultAsync<Department>(query, new { Id = id });
        }

        public async Task<Department?> GetByNameAsync(string name)
        {
            using var connection = _dapperDbContext.CreateConnection();
            const string query = @"SELECT * FROM ""Departments"" WHERE ""Name"" = @Name";
            return await connection.QueryFirstOrDefaultAsync<Department>(query, new { Name = name });
        }

        public async Task<Department> AddAsync(Department department)
        {
            using var connection = _dapperDbContext.CreateConnection();
            department.Id = Guid.NewGuid();
            const string query = @"
                INSERT INTO ""Departments"" (""Id"", ""Name"", ""Description"")
                VALUES (@Id, @Name, @Description)";
            await connection.ExecuteAsync(query, department);
            return department;
        }

        public async Task UpdateAsync(Department department)
        {
            using var connection = _dapperDbContext.CreateConnection();
            const string query = @"
                UPDATE ""Departments""
                SET ""Name"" = @Name, ""Description"" = @Description
                WHERE ""Id"" = @Id";
            await connection.ExecuteAsync(query, department);
        }

        public async Task DeleteAsync(Guid id)
        {
            using var connection = _dapperDbContext.CreateConnection();
            const string query = @"DELETE FROM ""Departments"" WHERE ""Id"" = @Id";
            await connection.ExecuteAsync(query, new { Id = id });
        }

        public async Task<int> TotalNumberOfDepartments()
        {
            using var connection = _dapperDbContext.CreateConnection();
            const string query = @"SELECT COUNT(*) FROM ""Departments""";
            return await connection.ExecuteScalarAsync<int>(query);
        }
    }
}
