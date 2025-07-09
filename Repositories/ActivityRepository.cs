using Dapper;
using Inventory_Mgmt_System.Data;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using System.Data;

namespace Inventory_Mgmt_System.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly DapperDbContext _dapperDbContext;

        public ActivityRepository(DapperDbContext dapperDbContext)
        {
            _dapperDbContext = dapperDbContext ?? throw new ArgumentNullException(nameof(dapperDbContext));
        }

        public async Task<List<Activity>> GetAllActivities()
        {
            using var connection = _dapperDbContext.CreateConnection();
            const string query = @"
                SELECT a.*, u.* 
                FROM ""Activity"" a 
                LEFT JOIN ""Users"" u ON a.""UserId"" = u.""Id""
                ORDER BY a.""Timestamp""";

            var activityDict = new Dictionary<Guid, Activity>();

            var result = await connection.QueryAsync<Activity, User, Activity>(
                query,
                (activity, user) =>
                {
                    if (!activityDict.TryGetValue(activity.Id, out var existingActivity))
                    {
                        existingActivity = activity;
                        activityDict.Add(existingActivity.Id, existingActivity);
                    }

                    existingActivity.User = user;
                    return existingActivity;
                });

            return result.Distinct().ToList();
        }

        public async Task<Activity> AddNewActivity(Activity activity)
        {
            if (activity == null)
                throw new ArgumentNullException(nameof(activity), "Activity cannot be null.");

            using var connection = _dapperDbContext.CreateConnection();
            activity.Id = activity.Id == Guid.Empty ? Guid.NewGuid() : activity.Id;
            activity.Timestamp = DateTime.UtcNow;

            const string insertQuery = @"
                INSERT INTO ""Activity"" (""Id"", ""Type"", ""Action"", ""Timestamp"", ""Status"", ""UserId"")
                VALUES (@Id, @Type, @Action, @Timestamp, @Status, @UserId)";

            await connection.ExecuteAsync(insertQuery, activity);
            return activity;
        }

        public async Task<Activity> DeleteActivity(Activity activity)
        {
            if (activity == null)
                throw new ArgumentNullException(nameof(activity), "Activity cannot be null.");

            using var connection = _dapperDbContext.CreateConnection();

            const string deleteQuery = @"DELETE FROM ""Activity"" WHERE ""Id"" = @Id";
            await connection.ExecuteAsync(deleteQuery, new { activity.Id });

            return activity;
        }

        public async Task<Activity> GetActivityById(Guid activityId)
        {
            using var connection = _dapperDbContext.CreateConnection();

            const string query = @"
                SELECT a.*, u.* 
                FROM ""Activity"" a 
                LEFT JOIN ""Users"" u ON a.""UserId"" = u.""Id""
                WHERE a.""Id"" = @Id";

            var result = await connection.QueryAsync<Activity, User, Activity>(
                query,
                (activity, user) =>
                {
                    activity.User = user;
                    return activity;
                },
                new { Id = activityId }
            );

            return result.FirstOrDefault();
        }
    }
}
