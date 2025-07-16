using Dapper;
using Inventory_Mgmt_System.Data;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using System.Data;

namespace Inventory_Mgmt_System.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DapperDbContext _dapperDbContext;

        public CategoryRepository(DapperDbContext dapperDbContext)
        {
            _dapperDbContext = dapperDbContext;
        }

        public async Task<Category> CreateCategory(Category category)
        {
            using var connection = _dapperDbContext.CreateConnection();
            category.Id = Guid.NewGuid();

            const string query = @"
                INSERT INTO ""Categories"" (""Id"", ""Name"", ""Description"", ""UserId"")
                VALUES (@Id, @Name, @Description, @UserId)";

            await connection.ExecuteAsync(query, category);
            return category;
        }

        public async Task<Category> GetCategoryById(Guid id)
        {
            using var connection = _dapperDbContext.CreateConnection();
            const string query = @"SELECT * FROM ""Categories"" WHERE ""Id"" = @Id";
            var category = await connection.QueryFirstOrDefaultAsync<Category>(query, new { Id = id });

            if (category == null)
                throw new KeyNotFoundException($"Category with ID {id} not found.");

            return category;
        }

        public async Task<Category> GetCategoryByName(string name)
        {
            using var connection = _dapperDbContext.CreateConnection();
            const string query = @"SELECT * FROM ""Categories"" WHERE ""Name"" = @Name";
            return await connection.QueryFirstOrDefaultAsync<Category>(query, new { Name = name });
        }

        public async Task<List<Category>> GetAllCategories()
        {
            using var connection = _dapperDbContext.CreateConnection();
            const string query = @"
                SELECT c.*, u.*
                FROM ""Categories"" c
                LEFT JOIN ""Users"" u ON c.""UserId"" = u.""Id""";

            var categoryDict = new Dictionary<Guid, Category>();

            var categories = await connection.QueryAsync<Category, User, Category>(
                query,
                (category, user) =>
                {
                    category.User = user;
                    return category;
                });

            return categories.Distinct().ToList();
        }

        public async Task<(List<Category> categories, int totalCount)> GetAllCategoriesByPaginationFilter(int page = 1, int pageSize = 6, string? search = null)
        {
            using var connection = _dapperDbContext.CreateConnection();

            page = Math.Max(page, 1);
            pageSize = Math.Max(pageSize, 1);
            var offset = (page - 1) * pageSize;

            // Query 1: Fetch paginated results
            const string sql = @"
        SELECT c.*, u.*
        FROM ""Categories"" c
        LEFT JOIN ""Users"" u ON c.""UserId"" = u.""Id""
        WHERE (@search IS NULL OR LOWER(c.""Name"") LIKE LOWER(@search))
        ORDER BY c.""Name""
        OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
    ";

            var categoryDict = new Dictionary<Guid, Category>();

            var categories = await connection.QueryAsync<Category, User, Category>(
                sql,
                (category, user) =>
                {
                    if (!categoryDict.TryGetValue(category.Id, out var existingCategory))
                    {
                        existingCategory = category;
                        categoryDict[category.Id] = existingCategory;
                    }

                    existingCategory.User = user;
                    return existingCategory;
                },
                new
                {
                    offset,
                    pageSize,
                    search = !string.IsNullOrEmpty(search) ? $"%{search}%" : null
                }
            );

            // Query 2: Get total count for pagination
            const string countQuery = @"
        SELECT COUNT(*) FROM ""Categories"" c
        WHERE (@search IS NULL OR LOWER(c.""Name"") LIKE LOWER(@search));
    ";

            var totalCount = await connection.ExecuteScalarAsync<int>(
                countQuery,
                new { search = !string.IsNullOrEmpty(search) ? $"%{search}%" : null }
            );

            return (categoryDict.Values.ToList(), totalCount);
        }



        public async Task<List<Category>> GetCategoryByUser(Guid userId)
        {
            using var connection = _dapperDbContext.CreateConnection();
            const string query = @"SELECT * FROM ""Categories"" WHERE ""UserId"" = @UserId";
            var result = await connection.QueryAsync<Category>(query, new { UserId = userId });
            return result.ToList();
        }

        public async Task<Category> UpdateCategory(Category updatedCategory)
        {
            using var connection = _dapperDbContext.CreateConnection();

            const string checkQuery = @"SELECT COUNT(*) FROM ""Categories"" WHERE ""Id"" = @Id";
            var exists = await connection.ExecuteScalarAsync<int>(checkQuery, new { updatedCategory.Id });
            if (exists == 0)
                throw new KeyNotFoundException($"Category with ID {updatedCategory.Id} not found.");

            const string updateQuery = @"
                UPDATE ""Categories""
                SET ""Name"" = @Name, ""Description"" = @Description, ""UserId"" = @UserId
                WHERE ""Id"" = @Id";

            await connection.ExecuteAsync(updateQuery, updatedCategory);
            return updatedCategory;
        }

        public async Task<Category> DeleteCategory(Guid id)
        {
            using var connection = _dapperDbContext.CreateConnection();

            var category = await GetCategoryById(id);

            const string deleteQuery = @"DELETE FROM ""Categories"" WHERE ""Id"" = @Id";
            await connection.ExecuteAsync(deleteQuery, new { Id = id });

            return category;
        }

        public async Task<int> TotalNumberOfCategory()
        {
            using var connection = _dapperDbContext.CreateConnection();
            const string query = @"SELECT COUNT(*) FROM ""Categories""";
            return await connection.ExecuteScalarAsync<int>(query);
        }
    }
}
