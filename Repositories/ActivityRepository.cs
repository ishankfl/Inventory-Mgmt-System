using Inventory_Mgmt_System.Data;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory_Mgmt_System.Repositories
{
    public class ActivityRepository:IActivityRepository
    {
        private readonly AppDbContext _dbContext;

        public ActivityRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<List<Activity>> GetAllActivities()
        {
            return await _dbContext.Activity
                .Include(a => a.User)  
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<Activity> AddNewActivity(Activity activity)
        {
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity), "Activity cannot be null.");
            }

            var entry = await _dbContext.Activity.AddAsync(activity);
            await _dbContext.SaveChangesAsync();

            return entry.Entity;
        }
        public async Task<Activity> DeleteActivity(Activity activity)
        {
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity), "Activity cannot be null.");
            }

            var entry =  _dbContext.Activity.Remove(activity);
            await _dbContext.SaveChangesAsync();

            return entry.Entity;
        }

       public async  Task<Activity> GetActivityById(Guid activityId)
        {
            var activity = _dbContext.Activity.Where(item=>item.Id == activityId).FirstOrDefault();
            return activity;
        }

    }
}
