using Inventory_Mgmt_System.Models;

namespace Inventory_Mgmt_System.Repositories.Interfaces
{
    public interface IActivityRepository
    {
        Task<List<Activity>> GetAllActivities();


        Task<Activity> AddNewActivity(Activity activity);

        Task<Activity> DeleteActivity(Activity activity);

        Task<Activity> GetActivityById(Guid activityId);

    }
}
