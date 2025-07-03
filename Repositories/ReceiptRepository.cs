using Dapper;
using Inventory_Mgmt_System.Data;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;

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
                string receiptsQuery = @"SELECT * FROM ""Receipts""";
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
            using (var connection = _dapperDbContext.CreateConnection())
            {
                connection.Open();
              
                    try
                    {
                        string updateReceiptQuery = @"
                    UPDATE ""Receipts"" 
                    SET 
                        ""ReceiptDate"" = @ReceiptDate,
                        ""BillNo"" = @BillNo,
                        ""VendorId"" = @VendorId
                    WHERE ""Id"" = @Id";

                        await connection.ExecuteAsync(updateReceiptQuery, new
                        {
                            receipt.Id,
                            receipt.ReceiptDate,
                            receipt.BillNo,
                            receipt.VendorId
                        });

                        string existingDetailsQuery = @"SELECT * FROM ""ReceiptDetails"" WHERE ""ReceiptId"" = @ReceiptId";
                        var existingDetails = (await connection.QueryAsync<ReceiptDetail>(
                            existingDetailsQuery, new { ReceiptId = receipt.Id })).ToList();

                        if (receipt.ReceiptDetails != null && receipt.ReceiptDetails.Any())
                        {
                            var detailsToKeep = receipt.ReceiptDetails.Where(rd => rd.Id != Guid.Empty).ToList();
                            var detailsToAdd = receipt.ReceiptDetails.Where(rd => rd.Id == Guid.Empty).ToList();

                            var detailsToDelete = existingDetails.Where(ed =>
                                !detailsToKeep.Any(rd => rd.Id == ed.Id)).ToList();

                            if (detailsToDelete.Any())
                            {
                                string deleteDetailsQuery = @"
                            DELETE FROM ""ReceiptDetails"" 
                            WHERE ""Id"" = @Id AND ""ReceiptId"" = @ReceiptId";

                                foreach (var detail in detailsToDelete)
                                {
                                    await connection.ExecuteAsync(deleteDetailsQuery, new
                                    {
                                        detail.Id,
                                        ReceiptId = receipt.Id
                                    });
                                }
                            }

                            string updateDetailQuery = @"
                        UPDATE ""ReceiptDetails"" 
                        SET 
                            ""ItemId"" = @ItemId,
                            ""Quantity"" = @Quantity,
                            ""Rate"" = @Rate
                        WHERE ""Id"" = @Id AND ""ReceiptId"" = @ReceiptId";

                            foreach (var detail in detailsToKeep)
                            {
                                await connection.ExecuteAsync(updateDetailQuery, new
                                {
                                    detail.Id,
                                    ReceiptId = receipt.Id,
                                    detail.ItemId,
                                    detail.Quantity,
                                    detail.Rate
                                });
                            }

                            string insertDetailQuery = @"
                        INSERT INTO ""ReceiptDetails"" 
                        (""Id"", ""ReceiptId"", ""ItemId"", ""Quantity"", ""Rate"") 
                        VALUES (@Id, @ReceiptId, @ItemId, @Quantity, @Rate)";

                            foreach (var detail in detailsToAdd)
                            {
                                detail.Id = Guid.NewGuid();
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
                        else
                        {
                            string deleteAllDetailsQuery = @"DELETE FROM ""ReceiptDetails"" WHERE ""ReceiptId"" = @ReceiptId";
                            await connection.ExecuteAsync(deleteAllDetailsQuery, new { ReceiptId = receipt.Id });
                        }


                        return await GetReceiptByIdAsync(receipt.Id);
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }
    }
}