using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;

namespace Inventory_Mgmt_System.Services.Interfaces
{
    public interface IActivityServices
    {
        Task<List<Activity>> GetAllActivities();


      

        Task<Activity> DeleteActivity(string activity);
        Task<Activity> AddNewActivity(ActivityDTO activityDto);


    }
}
