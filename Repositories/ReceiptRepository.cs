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

                try
                {
                    string insertReceiptQuery = @"
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
                    });

                    if (receipt.ReceiptDetails != null && receipt.ReceiptDetails.Any())
                    {
                        string insertDetailQuery = @"
                            INSERT INTO ""ReceiptDetails"" 
                            (""Id"", ""ReceiptId"", ""ItemId"", ""Quantity"", ""Rate"") 
                            VALUES (@Id, @ReceiptId, @ItemId, @Quantity, @Rate)";

                        foreach (var detail in receipt.ReceiptDetails)
                        {
                            detail.ReceiptId = receipt.Id;
                            await connection.ExecuteAsync(insertDetailQuery, new
                            {
                                detail.Id,
                                detail.ReceiptId,
                                detail.ItemId,
                                detail.Quantity,
                                detail.Rate
                            });
                        }
                    }

                    return receipt;
                }
                catch
                {
                    throw;
                }
            }
        }

        public async Task<Receipt> GetReceiptByIdAsync(Guid id)
        {
            using (var connection = _dapperDbContext.CreateConnection())
            {
                connection.Open();

                string receiptQuery = @"SELECT * FROM ""Receipts"" WHERE ""Id"" = @Id";
                var receipt = await connection.QueryFirstOrDefaultAsync<Receipt>(receiptQuery, new { Id = id });
                if (receipt == null) return null;

                string vendorQuery = @"SELECT * FROM ""Vendor"" WHERE ""Id"" = @VendorId";
                receipt.Vendor = await connection.QueryFirstOrDefaultAsync<Vendor>(
                    vendorQuery, new { VendorId = receipt.VendorId });

                string detailsQuery = @"SELECT * FROM ""ReceiptDetails"" WHERE ""ReceiptId"" = @Id";
                var details = (await connection.QueryAsync<ReceiptDetail>(
                    detailsQuery, new { Id = receipt.Id })).ToList();

                foreach (var detail in details)
                {
                    string itemQuery = @"SELECT * FROM ""Items"" WHERE ""Id"" = @ItemId";
                    detail.Item = await connection.QueryFirstOrDefaultAsync<Item>(
                        itemQuery, new { ItemId = detail.ItemId });

                    if (detail.Item != null)
                    {
                        string stockQuery = @"SELECT * FROM ""Stocks"" WHERE ""ItemId"" = @ItemId";
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
            using (var connection = _dapperDbContext.CreateConnection())
            {
                string receiptsQuery = @"SELECT * FROM ""Receipts"" ORDER BY ""ReceiptDate"" DESC";
                var receipts = (await connection.QueryAsync<Receipt>(receiptsQuery)).ToList();

                foreach (var receipt in receipts)
                {
                    // Fixed table name to "Vendors"
                    string vendorQuery = @"SELECT * FROM ""Vendor"" WHERE ""Id"" = @VendorId";
                    receipt.Vendor = await connection.QueryFirstOrDefaultAsync<Vendor>(
                        vendorQuery, new { VendorId = receipt.VendorId });
                    
                    string detailsQuery = @"SELECT * FROM ""ReceiptDetails"" WHERE ""ReceiptId"" = @ReceiptId";
                    var details = (await connection.QueryAsync<ReceiptDetail>(
                        detailsQuery, new { ReceiptId = receipt.Id })).ToList();

                    foreach (var detail in details)
                    {
                        string itemQuery = @"SELECT * FROM ""Items"" WHERE ""Id"" = @ItemId";
                        detail.Item = await connection.QueryFirstOrDefaultAsync<Item>(
                            itemQuery, new { ItemId = detail.ItemId });
                    }

                    receipt.ReceiptDetails = details;
                }

                return receipts;
            }
        }

        public async Task<bool> DeleteReceiptAsync(Guid id)
        {
            using (var connection = _dapperDbContext.CreateConnection())
            {
                string checkQuery = @"SELECT COUNT(*) FROM ""Receipts"" WHERE ""Id"" = @Id";
                var exists = await connection.ExecuteScalarAsync<int>(checkQuery, new { Id = id });
                if (exists == 0) return false;

                string deleteDetailsQuery = @"DELETE FROM ""ReceiptDetails"" WHERE ""ReceiptId"" = @ReceiptId";
                await connection.ExecuteAsync(deleteDetailsQuery, new { ReceiptId = id });

                string deleteReceiptQuery = @"DELETE FROM ""Receipts"" WHERE ""Id"" = @Id";
                await connection.ExecuteAsync(deleteReceiptQuery, new { Id = id });

                return true;
            }
        }
        public async Task<Receipt> UpdateReceiptAsync(Receipt receipt)
        {
            using var connection = _dapperDbContext.CreateConnection();
             connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                
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

                var existingDetails = (await connection.QueryAsync<ReceiptDetail>(
                    @"SELECT * FROM ""ReceiptDetails"" WHERE ""ReceiptId"" = @ReceiptId",
                    new { ReceiptId = receipt.Id },
                    transaction
                )).ToList();

                var incomingDetails = receipt.ReceiptDetails ?? new List<ReceiptDetail>();

                var detailsToUpdate = incomingDetails
                    .Where(rd => rd.Id != Guid.Empty && rd.Id != default && existingDetails.Any(ed => ed.Id == rd.Id))
                    .ToList();

                var detailsToInsert = incomingDetails
                    .Where(rd => rd.Id == Guid.Empty || rd.Id == default || !existingDetails.Any(ed => ed.Id == rd.Id))
                    .ToList();

                var detailsToDelete = existingDetails
                    .Where(ed => !incomingDetails.Any(rd => rd.Id == ed.Id))
                    .ToList();

                foreach (var detail in detailsToDelete)
                {
                    await connection.ExecuteAsync(
                        @"DELETE FROM ""ReceiptDetails"" WHERE ""Id"" = @Id AND ""ReceiptId"" = @ReceiptId",
                        new { detail.Id, ReceiptId = receipt.Id },
                        transaction
                    );

                    await AdjustStockAsync(connection, transaction, detail.ItemId, -detail.Quantity);
                }

                foreach (var detail in detailsToUpdate)
                {
                    var oldDetail = existingDetails.First(d => d.Id == detail.Id);
                    var quantityDiff = detail.Quantity - oldDetail.Quantity;

                    await connection.ExecuteAsync(
                        @"UPDATE ""ReceiptDetails"" 
                  SET ""ItemId"" = @ItemId, ""Quantity"" = @Quantity, ""Rate"" = @Rate
                  WHERE ""Id"" = @Id AND ""ReceiptId"" = @ReceiptId",
                        new { detail.Id, detail.ItemId, detail.Quantity, detail.Rate, ReceiptId = receipt.Id },
                        transaction
                    );

                    if (quantityDiff != 0)
                    {
                        await AdjustStockAsync(connection, transaction, detail.ItemId, quantityDiff);
                    }
                }

                foreach (var detail in detailsToInsert)
                {
                    detail.Id = Guid.NewGuid();
                    detail.ReceiptId = receipt.Id;

                    await connection.ExecuteAsync(
                        @"INSERT INTO ""ReceiptDetails"" (""Id"", ""ReceiptId"", ""ItemId"", ""Quantity"", ""Rate"")
                  VALUES (@Id, @ReceiptId, @ItemId, @Quantity, @Rate)",
                        detail,
                        transaction
                    );

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

        private async Task AdjustStockAsync(IDbConnection connection, IDbTransaction transaction, Guid itemId, decimal quantityDiff)
        {
            await connection.ExecuteAsync(
                @"UPDATE ""Stocks"" 
          SET ""CurrentQuantity"" = ""CurrentQuantity"" + @QuantityDiff
          WHERE ""ItemId"" = @ItemId",
                new { ItemId = itemId, QuantityDiff = quantityDiff },
                transaction
            );
        }


    }


}

