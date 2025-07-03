using Inventory_Mgmt_System.Data;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Dapper;
using System.Data;

namespace Inventory_Mgmt_System.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly DapperDbContext _dapperDbContext;

        public ItemRepository(DapperDbContext dapperDbContext)
        {
            _dapperDbContext = dapperDbContext;
        }

        public async Task<List<Item>> GetAllAsync()
        {
            using (var dbConnection = _dapperDbContext.CreateConnection())
            {
                dbConnection.Open();

                var query = @"SELECT ""Id"", ""Name"", ""Unit"" FROM ""Items"";";
                var items = await dbConnection.QueryAsync<Item>(query);
                return items.ToList();
            }
        }

        public async Task<Item> GetByIdAsync(Guid id)
        {
            using (var dbConnection = _dapperDbContext.CreateConnection())
            {
                dbConnection.Open();

                var query = @"SELECT ""Id"", ""Name"", ""Unit"" FROM ""Items"" WHERE ""Id"" = @Id;";
                var item = await dbConnection.QueryFirstOrDefaultAsync<Item>(query, new { Id = id });
                return item;
            }
        }

        public async Task<Item> AddAsync(Item item)
        {
            using (var dbConnection = _dapperDbContext.CreateConnection())
            {
                dbConnection.Open();

                const string query = @"
                    INSERT INTO ""Items"" (""Id"", ""Name"", ""Unit"")
                    VALUES (@Id, @Name, @Unit)
                    RETURNING ""Id"";";

                if (item.Id == Guid.Empty)
                {
                    item.Id = Guid.NewGuid();
                }

                var insertedId = await dbConnection.ExecuteScalarAsync<Guid>(query, item);
                return item;
            }
        }

        public async Task<Item> UpdateAsync(Item item)
        {
            using (var dbConnection = _dapperDbContext.CreateConnection())
            {
                dbConnection.Open();

                const string selectQuery = @"SELECT * FROM ""Items"" WHERE ""Id"" = @Id;";
                var existingItem = await dbConnection.QueryFirstOrDefaultAsync<Item>(selectQuery, new { item.Id });

                if (existingItem == null)
                {
                    throw new KeyNotFoundException($"Item with ID {item.Id} not found.");
                }

                const string updateQuery = @"
                    UPDATE ""Items""
                    SET ""Name"" = @Name, ""Unit"" = @Unit
                    WHERE ""Id"" = @Id;";

                await dbConnection.ExecuteAsync(updateQuery, item);
                return item;
            }
        }

        public async Task<Item> DeleteAsync(Guid id)
        {
            using (var dbConnection = _dapperDbContext.CreateConnection())
            {
                dbConnection.Open();

                const string selectQuery = @"SELECT * FROM ""Items"" WHERE ""Id"" = @Id;";
                var existingItem = await dbConnection.QueryFirstOrDefaultAsync<Item>(selectQuery, new { Id = id });

                if (existingItem == null)
                {
                    throw new KeyNotFoundException($"Item with ID {id} not found.");
                }

                const string deleteQuery = @"DELETE FROM ""Items"" WHERE ""Id"" = @Id;";
                await dbConnection.ExecuteAsync(deleteQuery, new { Id = id });

                return existingItem;
            }
        }
        public async Task<bool> Exists(Guid id)
        {
            using var dbConnection = _dapperDbContext.CreateConnection();

            const string query = @"SELECT COUNT(1) FROM ""Items"" WHERE ""Id"" = @Id;";
            return await dbConnection.ExecuteScalarAsync<bool>(query, new { Id = id });
        }

    }
}
