using Dapper;
using Inventory_Mgmt_System.Data;
using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using System.Data;

namespace Inventory_Mgmt_System.Repositories
{
    public class IssueRepository : IIssueRepository
    {
        private readonly DapperDbContext _dapperDbContext;

        public IssueRepository(DapperDbContext dapperDbContext)
        {
            _dapperDbContext = dapperDbContext;
        }
        public async Task<Issue> CreateIssueAsync(Issue issue)
        {
            using var connection = _dapperDbContext.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                const string insertIssueQuery = @"
            INSERT INTO ""Issues""
            (""Id"", ""IssueId"", ""IssueDate"", ""InvoiceNumber"", ""InvoiceDate"", ""DeliveryNote"", ""DepartmentId"", ""IssuedByUserId"")
            VALUES (@Id, @IssueId, @IssueDate, @InvoiceNumber, @InvoiceDate, @DeliveryNote, @DepartmentId, @IssuedByUserId)";

                await connection.ExecuteAsync(insertIssueQuery, new
                {
                    issue.Id,
                    issue.IssueId,
                    issue.IssueDate,
                    issue.InvoiceNumber,
                    issue.InvoiceDate,
                    issue.DeliveryNote,
                    issue.DepartmentId,
                    issue.IssuedByUserId
                }, transaction);

                if (issue.IssueDetails != null && issue.IssueDetails.Any())
                {
                    const string insertDetailQuery = @"
                INSERT INTO ""IssueDetails""
                (""Id"", ""IssueId"", ""ItemId"", ""Quantity"", ""IssueRate"")
                VALUES (@Id, @IssueId, @ItemId, @Quantity, @IssueRate)";

                    foreach (var detail in issue.IssueDetails)
                    {
                        detail.Id = detail.Id == Guid.Empty ? Guid.NewGuid() : detail.Id;
                        detail.IssueId = issue.Id;

                        await connection.ExecuteAsync(insertDetailQuery, new
                        {
                            detail.Id,
                            detail.IssueId,
                            detail.ItemId,
                            detail.Quantity,
                            detail.IssueRate
                            
                        }, transaction);

                        await AdjustStockAsync(connection, transaction, detail.ItemId, -detail.Quantity);
                    }
                }

                transaction.Commit();
                return issue;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }


        public async Task<Issue> GetIssueByIdAsync(Guid id)
        {
            using var connection = _dapperDbContext.CreateConnection();
            connection.Open();

            const string issueQuery = @"SELECT * FROM ""Issues"" WHERE ""Id"" = @Id";
            var issue = await connection.QueryFirstOrDefaultAsync<Issue>(issueQuery, new { Id = id });
            if (issue == null) return null;

            const string userQuery = @"SELECT * FROM ""Users"" WHERE ""Id"" = @UserId";
            issue.IssuedByUser = await connection.QueryFirstOrDefaultAsync<User>(userQuery, new { UserId = issue.IssuedByUserId });

            const string detailsQuery = @"SELECT * FROM ""IssueDetails"" WHERE ""IssueId"" = @IssueId";
            var details = (await connection.QueryAsync<IssueDetail>(
                detailsQuery, new { IssueId = id })).ToList();

            foreach (var detail in details)
            {
                const string itemQuery = @"SELECT * FROM ""Items"" WHERE ""Id"" = @ItemId";
                detail.Item = await connection.QueryFirstOrDefaultAsync<Item>(
                    itemQuery, new { ItemId = detail.ItemId });
                string stockQuery = @"SELECT * FROM ""Stock"" WHERE ""ItemId"" = @ItemId";

               var stockDetails = await  connection.QueryAsync<Stock>(stockQuery, new { ItemId = detail.ItemId });
                detail.Item.Stock = stockDetails.ToList();

            }

            issue.IssueDetails = details;

         
            return issue;
        }
        public async Task<IEnumerable<Issue>> GetAllIssuesAsync()
        {
            using var connection = _dapperDbContext.CreateConnection();
            connection.Open();

            const string issuesQuery = @"SELECT * FROM ""Issues"" ORDER BY ""IssueDate"" DESC";
            var issues = (await connection.QueryAsync<Issue>(issuesQuery)).ToList();

            if (!issues.Any()) return issues;

            var issuedUserIds = issues.Select(i => i.IssuedByUserId).Distinct().ToArray();
            const string usersQuery = @"SELECT * FROM ""Users"" WHERE ""Id"" = ANY(@UserIds)";
            var users = (await connection.QueryAsync<User>(usersQuery, new { UserIds = issuedUserIds }))
                        .ToDictionary(u => u.Id, u => u);

            var issueIds = issues.Select(i => i.Id).Distinct().ToArray();
            const string detailsQuery = @"SELECT * FROM ""IssueDetails"" WHERE ""IssueId"" = ANY(@IssueIds)";
            var details = (await connection.QueryAsync<IssueDetail>(detailsQuery, new { IssueIds = issueIds })).ToList();

            var itemIds = details.Select(d => d.ItemId).Distinct().ToArray();
            const string itemsQuery = @"SELECT * FROM ""Items"" WHERE ""Id"" = ANY(@ItemIds)";
            var items = (await connection.QueryAsync<Item>(itemsQuery, new { ItemIds = itemIds }))
                        .ToDictionary(i => i.Id, i => i);

            const string stocksQuery = @"SELECT * FROM ""Stock"" WHERE ""ItemId"" = ANY(@ItemIds)";
            var stocks = (await connection.QueryAsync<Stock>(stocksQuery, new { ItemIds = itemIds })).ToList();


            foreach (var issue in issues)
            {
                if (users.TryGetValue(issue.IssuedByUserId, out var user))
                    issue.IssuedByUser = user;
            }

            foreach (var issue in issues)
            {
                issue.IssueDetails = details
                    .Where(d => d.IssueId == issue.Id)
                    .ToList();
            }

            foreach (var detail in details)
            {
                if (items.TryGetValue(detail.ItemId, out var item))
                {
                    detail.Item = item;

                    var itemStocks = stocks.Where(s => s.ItemId == item.Id).ToList();
                    item.Stock = itemStocks;
                }
            }

            return issues;
        }

        public async Task<(List<Issue> Issues, int TotalCount)> GetAllPaginatedIssuesAsync(int page, int limit)
        {
            using var connection = _dapperDbContext.CreateConnection();
            connection.Open();

            int offset = (page - 1) * limit;

            const string paginatedIssuesQuery = @"
        SELECT * FROM ""Issues""
        ORDER BY ""IssueDate"" DESC
        OFFSET @Offset LIMIT @Limit;";

            const string countQuery = @"SELECT COUNT(*) FROM ""Issues"";";

            var issues = (await connection.QueryAsync<Issue>(paginatedIssuesQuery, new { Offset = offset, Limit = limit })).ToList();
            var totalCount = await connection.ExecuteScalarAsync<int>(countQuery);

            if (!issues.Any()) return (issues, totalCount);

            // 1. Load related users
            var issuedUserIds = issues.Select(i => i.IssuedByUserId).Distinct().ToArray();
            const string usersQuery = @"SELECT * FROM ""Users"" WHERE ""Id"" = ANY(@UserIds)";
            var users = (await connection.QueryAsync<User>(usersQuery, new { UserIds = issuedUserIds }))
                        .ToDictionary(u => u.Id, u => u);

            // 2. Load issue details
            var issueIds = issues.Select(i => i.Id).Distinct().ToArray();
            const string detailsQuery = @"SELECT * FROM ""IssueDetails"" WHERE ""IssueId"" = ANY(@IssueIds)";
            var details = (await connection.QueryAsync<IssueDetail>(detailsQuery, new { IssueIds = issueIds })).ToList();

            // 3. Load items
            var itemIds = details.Select(d => d.ItemId).Distinct().ToArray();
            const string itemsQuery = @"SELECT * FROM ""Items"" WHERE ""Id"" = ANY(@ItemIds)";
            var items = (await connection.QueryAsync<Item>(itemsQuery, new { ItemIds = itemIds }))
                        .ToDictionary(i => i.Id, i => i);

            // 4. Load stock
            const string stocksQuery = @"SELECT * FROM ""Stock"" WHERE ""ItemId"" = ANY(@ItemIds)";
            var stocks = (await connection.QueryAsync<Stock>(stocksQuery, new { ItemIds = itemIds })).ToList();

            // 5. Map Users
            foreach (var issue in issues)
            {
                if (users.TryGetValue(issue.IssuedByUserId, out var user))
                    issue.IssuedByUser = user;
            }

            // 6. Map IssueDetails
            foreach (var issue in issues)
            {
                issue.IssueDetails = details
                    .Where(d => d.IssueId == issue.Id)
                    .ToList();
            }

            // 7. Map Items and Stocks into IssueDetails
            foreach (var detail in details)
            {
                if (items.TryGetValue(detail.ItemId, out var item))
                {
                    detail.Item = item;

                    var itemStocks = stocks.Where(s => s.ItemId == item.Id).ToList();
                    item.Stock = itemStocks;
                }
            }

            return (issues, totalCount);
        }


        public async Task<bool> DeleteIssueAsync(Guid id)
        {
            using var connection = _dapperDbContext.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                const string checkQuery = @"SELECT COUNT(*) FROM ""Issues"" WHERE ""Id"" = @Id";
                var exists = await connection.ExecuteScalarAsync<int>(checkQuery, new { Id = id }, transaction);
                if (exists == 0) return false;

                const string getDetailsQuery = @"SELECT * FROM ""IssueDetails"" WHERE ""IssueId"" = @IssueId";
                var details = (await connection.QueryAsync<IssueDetail>(
                    getDetailsQuery, new { IssueId = id }, transaction)).ToList();

                foreach (var detail in details)
                {
                    await AdjustStockAsync(connection, transaction, detail.ItemId, detail.Quantity);
                }

                const string deleteDetailsQuery = @"DELETE FROM ""IssueDetails"" WHERE ""IssueId"" = @IssueId";
                await connection.ExecuteAsync(deleteDetailsQuery, new { IssueId = id }, transaction);

                const string deleteIssueQuery = @"DELETE FROM ""Issues"" WHERE ""Id"" = @Id";
                await connection.ExecuteAsync(deleteIssueQuery, new { Id = id }, transaction);

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public async Task<Issue> UpdateIssueAsync(Issue issue)
        {
            using var connection = _dapperDbContext.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Check if issue exists before updating
                var existingIssue = await connection.ExecuteScalarAsync<int>(
                    @"SELECT COUNT(*) FROM ""Issues"" WHERE ""Id"" = @Id",
                    new { issue.Id }, transaction);

                if (existingIssue == 0)
                {
                    throw new KeyNotFoundException($"Issue with Id {issue.Id} does not exist.");
                }

                const string updateIssueQuery = @"
        UPDATE ""Issues""
        SET ""IssueDate"" = @IssueDate,
            ""InvoiceNumber"" = @InvoiceNumber,
            ""InvoiceDate"" = @InvoiceDate,
            ""DeliveryNote"" = @DeliveryNote,
            ""DepartmentId"" = @DepartmentId,
            ""IssuedByUserId"" = @IssuedByUserId
        WHERE ""Id"" = @Id";

                await connection.ExecuteAsync(updateIssueQuery, new
                {
                    issue.Id,
                    issue.IssueDate,
                    issue.InvoiceNumber,
                    issue.InvoiceDate,
                    issue.DeliveryNote,
                    issue.DepartmentId,
                    issue.IssuedByUserId
                }, transaction);

                // Fetch existing details
                var existingDetails = (await connection.QueryAsync<IssueDetail>(
                    @"SELECT * FROM ""IssueDetails"" WHERE ""IssueId"" = @IssueId",
                    new { IssueId = issue.Id },
                    transaction
                )).ToList();

                var incomingDetails = issue.IssueDetails ?? new List<IssueDetail>();

                var detailsToUpdate = incomingDetails
                    .Where(d => d.Id != Guid.Empty && existingDetails.Any(ed => ed.Id == d.Id))
                    .ToList();

                var detailsToInsert = incomingDetails
                    .Where(d => d.Id == Guid.Empty || !existingDetails.Any(ed => ed.Id == d.Id))
                    .ToList();

                var detailsToDelete = existingDetails
                    .Where(ed => !incomingDetails.Any(d => d.Id == ed.Id))
                    .ToList();

                foreach (var detail in detailsToDelete)
                {
                    await connection.ExecuteAsync(
                        @"DELETE FROM ""IssueDetails"" WHERE ""Id"" = @Id AND ""IssueId"" = @IssueId",
                        new { detail.Id, IssueId = issue.Id },
                        transaction
                    );

                    await AdjustStockAsync(connection, transaction, detail.ItemId, detail.Quantity);
                }

                foreach (var detail in detailsToUpdate)
                {
                    var oldDetail = existingDetails.First(ed => ed.Id == detail.Id);
                    var quantityDiff = detail.Quantity - oldDetail.Quantity;

                    await connection.ExecuteAsync(
                        @"UPDATE ""IssueDetails""
                  SET ""ItemId"" = @ItemId,
                      ""Quantity"" = @Quantity,
                      ""IssueRate"" = @IssueRate
                  WHERE ""Id"" = @Id AND ""IssueId"" = @IssueId",
                        new
                        {
                            detail.Id,
                            detail.ItemId,
                            detail.Quantity,
                            detail.IssueRate,
                            IssueId = issue.Id
                        },
                        transaction
                    );

                    if (quantityDiff != 0)
                    {
                        await AdjustStockAsync(connection, transaction, detail.ItemId, -quantityDiff);
                    }
                }

                foreach (var detail in detailsToInsert)
                {
                    detail.Id = Guid.NewGuid();
                    detail.IssueId = issue.Id;

                    await connection.ExecuteAsync(
                        @"INSERT INTO ""IssueDetails"" (""Id"", ""IssueId"", ""ItemId"", ""Quantity"", ""IssueRate"")
                  VALUES (@Id, @IssueId, @ItemId, @Quantity, @IssueRate)",
                        detail,
                        transaction
                    );

                    await AdjustStockAsync(connection, transaction, detail.ItemId, -detail.Quantity);
                }

                transaction.Commit();
                return await GetIssueByIdAsync(issue.Id);
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
                @"UPDATE ""Stock"" 
                  SET ""CurrentQuantity"" = ""CurrentQuantity"" + @QuantityDiff
                  WHERE ""ItemId"" = @ItemId",
                new { ItemId = itemId, QuantityDiff = quantityDiff },
                transaction
            );
        }

        /*     public async Task<(Item Item, decimal TotalIssuedQuantity)> GetTopIssuedItemsAsync()
             {
                 using var connection = _dapperDbContext.CreateConnection();
                 connection.Open();

                 const string query = @"
             ";

                 var result = await connection.QueryAsync<Item, decimal, (Item, decimal)>(
                     query,
                     (item, totalQty) => (item, totalQty),
                     splitOn: "TotalIssuedQuantity"
                 );

                 return result.ToList();
             }
     */



        /* public async Task<List<(Item Item, decimal TotalIssuedQuantity)>> GetTopIssuedItemsAsync()
         {
             using var connection = _dapperDbContext.CreateConnection();

             const string query = @"
     SELECT 
         i.""Id"",
         i.""Name"",

         SUM(d.""Quantity"") AS TotalIssuedQuantity
     FROM ""IssueDetails"" d
     INNER JOIN ""Items"" i ON i.""Id"" = d.""ItemId""
     GROUP BY i.""Id"", i.""Name""
     ORDER BY TotalIssuedQuantity DESC
     LIMIT 10";

             var rawResult = await connection.QueryAsync<TopIssuedItemDto>(query);

             return rawResult
                 .Select(r => (
                     new Item
                     {
                         Id = r.Id,
                         Name = r.Name,
                     },
                     r.TotalIssuedQuantity
                 ))
                 .ToList();
         }*/

        public async Task<List<TopIssuedItemResponseDto>> GetTopIssuedItemsAsync()
        {
            using var connection = _dapperDbContext.CreateConnection();

            const string query = @"
        SELECT 
            i.""Id"" AS ItemId,
            i.""Name"",
            SUM(d.""Quantity"") AS TotalIssuedQuantity
        FROM ""IssueDetails"" d
        INNER JOIN ""Items"" i ON i.""Id"" = d.""ItemId""
        GROUP BY i.""Id"", i.""Name""
        ORDER BY TotalIssuedQuantity DESC
        LIMIT 10";

            var result = await connection.QueryAsync<TopIssuedItemResponseDto>(query);
            return result.ToList();
        }



    }
}
