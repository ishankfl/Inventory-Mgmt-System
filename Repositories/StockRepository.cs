using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Inventory_Mgmt_System.Data;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Inventory_Mgmt_System.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly DapperDbContext _dapperContext;
        private readonly ILogger<StockRepository> _logger;

        public StockRepository(DapperDbContext dapperContext, ILogger<StockRepository> logger)
        {
            _dapperContext = dapperContext;
            _logger = logger;
        }

        public async Task<List<Stock>> GetAllStocksAsync()
        {
            using (var dbConnection = _dapperContext.CreateConnection())
            {
                dbConnection.Open();
                var query = @"
                    SELECT s.*, i.* 
                    FROM ""Stock"" s
                    INNER JOIN ""Items"" i ON s.""ItemId"" = i.""Id""";

                var stocks = await dbConnection.QueryAsync<Stock, Item, Stock>(
                    query,
                    (stock, item) => {
                        stock.Item = item;
                        return stock;
                    },
                    splitOn: "Id");

                return stocks.AsList();
            }
        }

        public async Task<Stock> GetStockByIdAsync(Guid id)
        {
            using (var dbConnection = _dapperContext.CreateConnection())
            {
                dbConnection.Open();
                var query = @"
                    SELECT s.*, i.* 
                    FROM ""Stock"" s
                    INNER JOIN ""Items"" i ON s.""ItemId"" = i.""Id""
                    WHERE s.""Id"" = @Id";

                var result = await dbConnection.QueryAsync<Stock, Item, Stock>(
                    query,
                    (stock, item) => {
                        stock.Item = item;
                        return stock;
                    },
                    new { Id = id },
                    splitOn: "Id");

                return result.FirstOrDefault();
            }
        }

        public async Task<Stock> GetStockByItemIdAsync(Guid itemId)
        {
            using (var dbConnection = _dapperContext.CreateConnection())
            {
                dbConnection.Open();
                var query = @"
                    SELECT s.*, i.* 
                    FROM ""Stock"" s
                    INNER JOIN ""Items"" i ON s.""ItemId"" = i.""Id""
                    WHERE s.""ItemId"" = @ItemId";

                var result = await dbConnection.QueryAsync<Stock, Item, Stock>(
                    query,
                    (stock, item) => {
                        stock.Item = item;
                        return stock;
                    },
                    new { ItemId = itemId },
                    splitOn: "Id");

                return result.FirstOrDefault();
            }
        }

        public async Task<Stock> AddStockAsync(Stock stock)
        {
            using (var dbConnection = _dapperContext.CreateConnection())
            {
                dbConnection.Open();
                var query = @"
                    INSERT INTO ""Stock"" (""Id"", ""ItemId"", ""CurrentQuantity"")
                    VALUES (@Id, @ItemId, @CurrentQuantity)
                    RETURNING ""Id""";

                var stockId = await dbConnection.ExecuteScalarAsync<Guid>(query, new
                {
                    stock.Id,
                    stock.ItemId,
                    stock.CurrentQuantity
                });

                return await GetStockByIdAsync(stockId);
            }
        }

        public async Task<Stock> UpdateStockAsync(Stock stock)
        {
            using (var dbConnection = _dapperContext.CreateConnection())
            {
                dbConnection.Open();
                var query = @"
                    UPDATE ""Stock""
                    SET ""CurrentQuantity"" = @CurrentQuantity
                    WHERE ""Id"" = @Id";

                await dbConnection.ExecuteAsync(query, new
                {
                    stock.CurrentQuantity,
                    stock.Id
                });

                return await GetStockByIdAsync(stock.Id);
            }
        }

        public async Task<bool> DeleteStockAsync(Guid id)
        {
            using (var dbConnection = _dapperContext.CreateConnection())
            {
                dbConnection.Open();
                var query = @"DELETE FROM ""Stock"" WHERE ""Id"" = @Id";
                var affectedRows = await dbConnection.ExecuteAsync(query, new { Id = id });
                return affectedRows > 0;
            }
        }

        public async Task<decimal> GetCurrentQuantityByItemIdAsync(Guid itemId)
        {
            using (var dbConnection = _dapperContext.CreateConnection())
            {
                dbConnection.Open();
                var query = @"SELECT ""CurrentQuantity"" FROM ""Stock"" WHERE ""ItemId"" = @ItemId";
                return await dbConnection.ExecuteScalarAsync<decimal>(query, new { ItemId = itemId });
            }
        }
    }
}