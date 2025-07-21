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
        /*
                public async Task<List<Item>> GetAllAsync()
                {
                    using (var dbConnection = _dapperDbContext.CreateConnection())
                    {
                        dbConnection.Open();

                        var query = @"SELECT *  FROM ""Items"";";
                        var stockQuery = @"SELECT * FROM ""Stock"" WHERE ""ItemId"" = @ItemId;";

                        var items = (await dbConnection.QueryAsync<Item>(query)).ToList();

                        foreach (var item in items)
                        {
                            var stocks = await dbConnection.QueryAsync<Stock>(stockQuery, new { ItemId = item.Id });
                            item.Stock = stocks.ToList(); // Assuming Item has a List<Stock> Stocks property
                        }

                        return items;
                    }
                }*/
        public async Task<List<Item>> GetAllItemNamesAndIdsAsync()
        {
            using (var dbConnection = _dapperDbContext.CreateConnection())
            {
                dbConnection.Open();

                var itemQuery = @"SELECT * FROM ""Items"" ORDER BY ""Name"";";
                var stockQuery = @"SELECT * FROM ""Stock"" WHERE ""ItemId"" = @ItemId;";

                var items = (await dbConnection.QueryAsync<Item>(itemQuery)).ToList();

                foreach (var item in items)
                {
                    var stock = await dbConnection.QueryAsync<Stock>(stockQuery, new { ItemId = item.Id });
                    item.Stock = stock.ToList();
                }

                return items;
            }
        }

        public async Task<(List<Item> Items, int TotalCount)> GetAllPaginatedAsync(int page, int limit, string search = "")
        {
            using (var dbConnection = _dapperDbContext.CreateConnection())
            {
                dbConnection.Open();

                int offset = (page - 1) * limit;

                var query = @"
            SELECT * FROM ""Items""
            WHERE LOWER(""Name"") LIKE LOWER('%' || @Search || '%')
            ORDER BY ""CreatedDate"" DESC
            OFFSET @Offset LIMIT @Limit;";

                var countQuery = @"
            SELECT COUNT(*) FROM ""Items""
            WHERE LOWER(""Name"") LIKE LOWER('%' || @Search || '%');";

                var stockQuery = @"SELECT * FROM ""Stock"" WHERE ""ItemId"" = @ItemId;";

                var items = (await dbConnection.QueryAsync<Item>(query, new
                {
                    Offset = offset,
                    Limit = limit,
                    Search = search ?? ""
                })).ToList();

                var totalCount = await dbConnection.ExecuteScalarAsync<int>(countQuery, new
                {
                    Search = search ?? ""
                });

                foreach (var item in items)
                {
                    var stocks = await dbConnection.QueryAsync<Stock>(stockQuery, new { ItemId = item.Id });
                    item.Stock = stocks.ToList();
                }

                return (items, totalCount);
            }
        }




        public async Task<Item> GetByIdAsync(Guid id)
        {
            using (var dbConnection = _dapperDbContext.CreateConnection())
            {
                dbConnection.Open();

                var query = @"SELECT * FROM ""Items"" WHERE ""Id"" = @Id;";
                var stockQuery = @"SELECT * FROM ""Stock"" WHERE ""ItemId"" = @ItemId";
                var item = await dbConnection.QueryFirstOrDefaultAsync<Item>(query, new { Id = id });
                if (item != null)
                {
                    var stock = await dbConnection.QueryAsync<Stock>(stockQuery, new { ItemId = id });
                    item.Stock = stock.ToList();
                }
                return item;
            }
        }

        public async Task<int> GetTotalCountOfItems()
        {
            using (var dbConnection = _dapperDbContext.CreateConnection())
            {
                dbConnection.Open();
                var query = @"SELECT  COUNT(*) FROM ""Items""";
                //SELECT distinct Count(*) FROM "Items"  "Name";
                var count = await dbConnection.QuerySingleAsync<int>(query);
                return count;

            }
            return 0;
        }

        public async Task<Item> AddAsync(Item item)
        {
            using (var dbConnection = _dapperDbContext.CreateConnection())
            {
                dbConnection.Open();

                const string query = @"
                    INSERT INTO ""Items"" (""Id"", ""Name"", ""Unit"", ""Price"")
                    VALUES (@Id, @Name, @Unit, @Price)
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
                    SET ""Name"" = @Name, ""Unit"" = @Unit, ""Price"" = @Price
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
