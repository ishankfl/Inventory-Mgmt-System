using Inventory_Mgmt_System.Models;

namespace Inventory_Mgmt_System.Services.Interfaces
{
    public interface IActivityServices
    {
        Task<List<Activity>> GetAllActivities();


        Task<Activity> AddNewActivity(Activity activity);

        Task<Activity> DeleteActivity(string activity);


    }
}
