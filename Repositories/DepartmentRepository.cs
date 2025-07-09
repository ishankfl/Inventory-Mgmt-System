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
