using Dapper;
using Inventory_Mgmt_System.Data;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Server.Models;
using Server.Services.Data;
using System.Data;

namespace Server.Services.Repositories.ProductServices
{
    public class ProductRepository : IProductRepository
    {
        private readonly DapperDbContext _dapperDbContext;

        public ProductRepository(DapperDbContext dapperDbContext)
        {
            _dapperDbContext = dapperDbContext;
        }

        public async Task<Product> CreateProduct(Product product)
        {
            using (var db = _dapperDbContext.CreateConnection())
            {
                string query = @"
                    INSERT INTO ""Products""
                        (""Id"", ""Name"", ""Description"", ""Quantity"", ""Price"", ""CategoryId"", ""UserId"", ""CreatedAt"")
                    VALUES
                        (@Id, @Name, @Description, @Quantity, @Price, @CategoryId, @UserId, @CreatedAt)
                    RETURNING *;";

                product.Id = Guid.NewGuid();
                product.CreatedAt = DateTime.UtcNow;

                return await db.QuerySingleAsync<Product>(query, product);
            }
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            using (var db = _dapperDbContext.CreateConnection())
            {
                string query = @"SELECT * FROM ""Products"" ORDER BY ""CreatedAt"" DESC;";
                return await db.QueryAsync<Product>(query);
            }
        }

        public async Task<Product?> GetProductById(Guid id)
        {
            using (var db = _dapperDbContext.CreateConnection())
            {
                string query = @"SELECT * FROM ""Products"" WHERE ""Id"" = @Id;";
                return await db.QuerySingleOrDefaultAsync<Product>(query, new { Id = id });
            }
        }

        public async Task<IEnumerable<Product>> GetProductsByCategory(Guid categoryId)
        {
            using (var db = _dapperDbContext.CreateConnection())
            {
                string query = @"SELECT * FROM ""Products"" WHERE ""CategoryId"" = @CategoryId;";
                return await db.QueryAsync<Product>(query, new { CategoryId = categoryId });
            }
        }

        public async Task<IEnumerable<Product>> GetProductsByUser(Guid userId)
        {
            using (var db = _dapperDbContext.CreateConnection())
            {
                string query = @"SELECT * FROM ""Products"" WHERE ""UserId"" = @UserId;";
                return await db.QueryAsync<Product>(query, new { UserId = userId });
            }
        }

        public async Task<Product?> UpdateProduct(Product product)
        {
            using (var db = _dapperDbContext.CreateConnection())
            {
                string query = @"
                    UPDATE ""Products""
                    SET 
                        ""Name"" = @Name,
                        ""Description"" = @Description,
                        ""Quantity"" = @Quantity,
                        ""Price"" = @Price,
                        ""CategoryId"" = @CategoryId,
                        ""UserId"" = @UserId
                    WHERE ""Id"" = @Id
                    RETURNING *;";

                return await db.QuerySingleOrDefaultAsync<Product>(query, product);
            }
        }

        public async Task<bool> DeleteProduct(Guid id)
        {
            using (var db = _dapperDbContext.CreateConnection())
            {
                string query = @"DELETE FROM ""Products"" WHERE ""Id"" = @Id;";
                int affectedRows = await db.ExecuteAsync(query, new { Id = id });
                return affectedRows > 0;
            }
        }

        public async Task<IEnumerable<Product>> GetTopTenProductsByQty()
        {
            using (var db = _dapperDbContext.CreateConnection())
            {
                string query = @"
                    SELECT * FROM ""Products""
                    ORDER BY ""Quantity"" DESC
                    LIMIT 10;";
                return await db.QueryAsync<Product>(query);
            }
        }

        public async Task<int> TotalNumberOfProduct()
        {
            using (var db = _dapperDbContext.CreateConnection())
            {
                string query = @"SELECT COUNT(*) FROM ""Products"";";
                return await db.ExecuteScalarAsync<int>(query);
            }
        }
    }
}
