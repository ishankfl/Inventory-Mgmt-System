using Dapper;
using Inventory_Mgmt_System.Data;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using System.Data;

namespace Inventory_Mgmt_System.Repositories
{
    public class ReceiptRepository : IReceiptRepository
    {
        private readonly DapperDbContext _dapperDbContext;

        public ReceiptRepository(DapperDbContext dapperDbContext)
        {
            _dapperDbContext = dapperDbContext;
        }

        public async Task<Receipt> CreateReceiptAsync(Receipt receipt)
        {
            using (var connection = _dapperDbContext.CreateConnection())
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();

                try
                {
                    const string insertReceiptQuery = @"
                        INSERT INTO ""Receipts"" 
                        (""Id"", ""ReceiptId"", ""ReceiptDate"", ""BillNo"", ""VendorId"") 
                        VALUES (@Id, @ReceiptId, @ReceiptDate, @BillNo, @VendorId)";

                    await connection.ExecuteAsync(insertReceiptQuery, new
                    {
                        receipt.Id,
                        receipt.ReceiptId,
                        receipt.ReceiptDate,
                        receipt.BillNo,
                        receipt.VendorId
                    }, transaction);

                    if (receipt.ReceiptDetails != null && receipt.ReceiptDetails.Any())
                    {
                        const string insertDetailQuery = @"
        INSERT INTO ""ReceiptDetails"" 
        (""Id"", ""ReceiptId"", ""ItemId"", ""Quantity"", ""Rate"") 
        VALUES (@Id, @ReceiptId, @ItemId, @Quantity, @Rate)";

                        const string updateItemQuery = @"
        UPDATE ""Items"" 
        SET ""Price"" = @Price 
        WHERE ""Id"" = @Id";

                        foreach (var detail in receipt.ReceiptDetails)
                        {
                            detail.Id = Guid.NewGuid();
                            detail.ReceiptId = receipt.Id;

                            await connection.ExecuteAsync(insertDetailQuery, detail, transaction);

                            await connection.ExecuteAsync(updateItemQuery, new
                            {
                                Price = detail.Rate,
                                Id = detail.ItemId
                            }, transaction);

                            // Update stock
                            await AdjustStockAsync(connection, transaction, detail.ItemId, detail.Quantity);
                        }
                    }


                    transaction.Commit();
                    return receipt;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task<Receipt> GetReceiptByIdAsync(Guid id)
        {
            using (var connection = _dapperDbContext.CreateConnection())
            {
                connection.Open();

                // Get receipt
                const string receiptQuery = @"SELECT * FROM ""Receipts"" WHERE ""Id"" = @Id";
                var receipt = await connection.QueryFirstOrDefaultAsync<Receipt>(receiptQuery, new { Id = id });
                if (receipt == null) return null;

                // Get vendor
                const string vendorQuery = @"SELECT * FROM ""Vendor"" WHERE ""Id"" = @VendorId";
                receipt.Vendor = await connection.QueryFirstOrDefaultAsync<Vendor>(
                    vendorQuery, new { VendorId = receipt.VendorId });

                // Get receipt details
                const string detailsQuery = @"SELECT * FROM ""ReceiptDetails"" WHERE ""ReceiptId"" = @ReceiptId";
                var details = (await connection.QueryAsync<ReceiptDetail>(
                    detailsQuery, new { ReceiptId = receipt.Id })).ToList();

                foreach (var detail in details)
                {
                    // Get item for each detail
                    const string itemQuery = @"SELECT * FROM ""Items"" WHERE ""Id"" = @ItemId";
                    detail.Item = await connection.QueryFirstOrDefaultAsync<Item>(
                        itemQuery, new { ItemId = detail.ItemId });

                    if (detail.Item != null)
                    {
                        // Get stock for each item
                        const string stockQuery = @"SELECT * FROM ""Stock"" WHERE ""ItemId"" = @ItemId";
                        var stocks = (await connection.QueryAsync<Stock>(
                            stockQuery, new { ItemId = detail.Item.Id })).ToList();

                        detail.Item.Stock = stocks;
                    }
                }

                receipt.ReceiptDetails = details;
                return receipt;
            }
        }

        public async Task<IEnumerable<Receipt>> GetAllReceiptsAsync()
        {
            using var connection = _dapperDbContext.CreateConnection();
            connection.Open();

            const string receiptQuery = @"SELECT * FROM ""Receipts"" ORDER BY ""ReceiptDate"" DESC";
            var receipts = (await connection.QueryAsync<Receipt>(receiptQuery)).ToList();

            foreach (var receipt in receipts)
            {
                // Get Vendor for each receipt
                const string vendorQuery = @"SELECT * FROM ""Vendor"" WHERE ""Id"" = @VendorId";
                receipt.Vendor = await connection.QueryFirstOrDefaultAsync<Vendor>(
                    vendorQuery, new { VendorId = receipt.VendorId });

                // Get ReceiptDetails for each receipt
                const string detailQuery = @"SELECT * FROM ""ReceiptDetails"" WHERE ""ReceiptId"" = @ReceiptId";
                var details = (await connection.QueryAsync<ReceiptDetail>(
                    detailQuery, new { ReceiptId = receipt.Id })).ToList();

                foreach (var detail in details)
                {
                    // Get Item for each detail
                    const string itemQuery = @"SELECT * FROM ""Items"" WHERE ""Id"" = @ItemId";
                    detail.Item = await connection.QueryFirstOrDefaultAsync<Item>(
                        itemQuery, new { ItemId = detail.ItemId });

                    // Get Stock for each item
                    if (detail.Item != null)
                    {
                        const string stockQuery = @"SELECT * FROM ""Stock"" WHERE ""ItemId"" = @ItemId";
                        var stocks = (await connection.QueryAsync<Stock>(
                            stockQuery, new { ItemId = detail.Item.Id })).ToList();

                        detail.Item.Stock = stocks;
                    }
                }

                receipt.ReceiptDetails = details;
            }

            return receipts;
        }


        public async Task<bool> DeleteReceiptAsync(Guid id)
        {
            using (var connection = _dapperDbContext.CreateConnection())
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();

                try
                {
                    // First get all details to adjust stock
                    const string getDetailsQuery = @"SELECT * FROM ""ReceiptDetails"" WHERE ""ReceiptId"" = @ReceiptId";
                    var details = (await connection.QueryAsync<ReceiptDetail>(
                        getDetailsQuery, new { ReceiptId = id }, transaction)).ToList();

                    // Adjust stock for each item
                    foreach (var detail in details)
                    {
                        await AdjustStockAsync(connection, transaction, detail.ItemId, -detail.Quantity);
                    }

                    // Delete details
                    const string deleteDetailsQuery = @"DELETE FROM ""ReceiptDetails"" WHERE ""ReceiptId"" = @ReceiptId";
                    await connection.ExecuteAsync(deleteDetailsQuery, new { ReceiptId = id }, transaction);

                    // Delete receipt
                    const string deleteReceiptQuery = @"DELETE FROM ""Receipts"" WHERE ""Id"" = @Id";
                    int affectedRows = await connection.ExecuteAsync(deleteReceiptQuery, new { Id = id }, transaction);

                    transaction.Commit();
                    return affectedRows > 0;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task<Receipt> UpdateReceiptAsync(Receipt receipt)
        {
            using (var connection = _dapperDbContext.CreateConnection())
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();

                try
                {
                    // Update receipt
                    const string updateReceiptQuery = @"
                        UPDATE ""Receipts"" 
                        SET ""ReceiptDate"" = @ReceiptDate,
                            ""BillNo"" = @BillNo,
                            ""VendorId"" = @VendorId
                        WHERE ""Id"" = @Id";

                    await connection.ExecuteAsync(updateReceiptQuery, new
                    {
                        receipt.Id,
                        receipt.ReceiptDate,
                        receipt.BillNo,
                        receipt.VendorId
                    }, transaction);

                    // Get existing details
                    const string existingDetailsQuery = @"SELECT * FROM ""ReceiptDetails"" WHERE ""ReceiptId"" = @ReceiptId";
                    var existingDetails = (await connection.QueryAsync<ReceiptDetail>(
                        existingDetailsQuery, new { ReceiptId = receipt.Id }, transaction)).ToList();

                    var incomingDetails = receipt.ReceiptDetails ?? new List<ReceiptDetail>();

                    // Process details to be deleted
                    var detailsToDelete = existingDetails
                        .Where(ed => !incomingDetails.Any(id => id.Id == ed.Id))
                        .ToList();

                    foreach (var detail in detailsToDelete)
                    {
                        await connection.ExecuteAsync(
                            @"DELETE FROM ""ReceiptDetails"" WHERE ""Id"" = @Id",
                            new { detail.Id },
                            transaction);

                        await AdjustStockAsync(connection, transaction, detail.ItemId, -detail.Quantity);
                    }

                    // Process details to be updated
                    var detailsToUpdate = incomingDetails
                        .Where(id => id.Id != Guid.Empty && existingDetails.Any(ed => ed.Id == id.Id))
                        .ToList();

                    foreach (var detail in detailsToUpdate)
                    {
                        var oldDetail = existingDetails.First(ed => ed.Id == detail.Id);
                        var quantityDiff = detail.Quantity - oldDetail.Quantity;

                        await connection.ExecuteAsync(
                            @"UPDATE ""ReceiptDetails"" 
                            SET ""ItemId"" = @ItemId, 
                                ""Quantity"" = @Quantity, 
                                ""Rate"" = @Rate
                            WHERE ""Id"" = @Id",
                            new
                            {
                                detail.Id,
                                detail.ItemId,
                                detail.Quantity,
                                detail.Rate
                            },
                            transaction);

                        if (quantityDiff != 0)
                        {
                            await AdjustStockAsync(connection, transaction, detail.ItemId, quantityDiff);
                        }
                    }

                    // Process details to be inserted
                    var detailsToInsert = incomingDetails
                        .Where(id => id.Id == Guid.Empty || !existingDetails.Any(ed => ed.Id == id.Id))
                        .ToList();

                    foreach (var detail in detailsToInsert)
                    {
                        detail.Id = Guid.NewGuid();
                        detail.ReceiptId = receipt.Id;

                        await connection.ExecuteAsync(
                            @"INSERT INTO ""ReceiptDetails"" 
                            (""Id"", ""ReceiptId"", ""ItemId"", ""Quantity"", ""Rate"")
                            VALUES (@Id, @ReceiptId, @ItemId, @Quantity, @Rate)",
                            detail,
                            transaction);

                        await AdjustStockAsync(connection, transaction, detail.ItemId, detail.Quantity);
                    }

                    transaction.Commit();
                    return await GetReceiptByIdAsync(receipt.Id);
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        private async Task AdjustStockAsync(IDbConnection connection, IDbTransaction transaction, Guid itemId, decimal quantityDiff)
        {
            const string updateStockQuery = @"
                UPDATE ""Stock"" 
                SET ""CurrentQuantity"" = ""CurrentQuantity"" + @QuantityDiff
                WHERE ""ItemId"" = @ItemId";

            await connection.ExecuteAsync(
                updateStockQuery,
                new { ItemId = itemId, QuantityDiff = quantityDiff },
                transaction);
        }

        public async Task<IEnumerable<ReceiptDetail>> GetAllReceiptDetailsAsync()
        {
            using (var connection = _dapperDbContext.CreateConnection())
            {
                connection.Open();

                // Get all receipt details
                const string detailsQuery = @"SELECT * FROM ""ReceiptDetails""";
                var details = (await connection.QueryAsync<ReceiptDetail>(detailsQuery)).ToList();

                // Get all related receipts
                var receiptIds = details.Select(d => d.ReceiptId).Distinct().ToList();
                const string receiptsQuery = @"SELECT * FROM ""Receipts"" WHERE ""Id"" IN @ReceiptIds";
                var receipts = (await connection.QueryAsync<Receipt>(
                    receiptsQuery, new { ReceiptIds = receiptIds })).ToDictionary(r => r.Id);

                // Get all related vendors
                var vendorIds = receipts.Values.Select(r => r.VendorId).Distinct().ToList();
                const string vendorsQuery = @"SELECT * FROM ""Vendor"" WHERE ""Id"" IN @VendorIds";
                var vendors = (await connection.QueryAsync<Vendor>(
                    vendorsQuery, new { VendorIds = vendorIds })).ToDictionary(v => v.Id);

                // Get all related items
                var itemIds = details.Select(d => d.ItemId).Distinct().ToList();
                const string itemsQuery = @"SELECT * FROM ""Items"" WHERE ""Id"" IN @ItemIds";
                var items = (await connection.QueryAsync<Item>(
                    itemsQuery, new { ItemIds = itemIds })).ToDictionary(i => i.Id);

                // Get all related stocks
                const string stocksQuery = @"SELECT * FROM ""Stock"" WHERE ""ItemId"" IN @ItemIds";
                var allStocks = (await connection.QueryAsync<Stock>(
                    stocksQuery, new { ItemIds = itemIds })).ToList();
                var stocksByItem = allStocks.GroupBy(s => s.ItemId)
                    .ToDictionary(g => g.Key, g => g.AsEnumerable());

                // Build the object graph
                foreach (var detail in details)
                {
                    if (receipts.TryGetValue(detail.ReceiptId, out var receipt))
                    {
                        detail.Receipt = receipt;
                        if (vendors.TryGetValue(receipt.VendorId, out var vendor))
                        {
                            receipt.Vendor = vendor;
                        }
                    }

                    if (items.TryGetValue(detail.ItemId, out var item))
                    {
                        detail.Item = item;
                        if (stocksByItem.TryGetValue(item.Id, out var stocks))
                        {
                            item.Stock = stocks.ToList();
                        }
                    }
                }

                return details.OrderByDescending(d => d.Receipt?.ReceiptDate);
            }
        }

        public async Task<IEnumerable<ReceiptDetail>> GetSimplifiedReceiptDetailsAsync()
        {
            using (var connection = _dapperDbContext.CreateConnection())
            {
                const string query = @"
                    SELECT 
                        rd.""Id"",
                        rd.""Quantity"",
                        rd.""Rate"",
                        r.""ReceiptId"",
                        r.""ReceiptDate"",
                        r.""BillNo"",
                        v.""Name"" as VendorName,
                        i.""Name"" as ItemName,
                        i.""Unit""
                    FROM ""ReceiptDetails"" rd
                    JOIN ""Receipts"" r ON rd.""ReceiptId"" = r.""Id""
                    JOIN ""Vendor"" v ON r.""VendorId"" = v.""Id""
                    JOIN ""Items"" i ON rd.""ItemId"" = i.""Id""
                    ORDER BY r.""ReceiptDate"" DESC";

                return await connection.QueryAsync<ReceiptDetail>(query);
            }
        }
    }
}