using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Inventory_Mgmt_System.Services.Interfaces;

namespace Inventory_Mgmt_System.Services
{
    public class ActivityService:IActivityServices
    {
        private readonly IActivityRepository _repository;
        public ActivityService(IActivityRepository repository)
        {
            _repository = repository;
        }

         public async Task<List<Activity>> GetAllActivities() { 
            var activities = await _repository.GetAllActivities();
            return activities;
        }


        public async Task<Activity> AddNewActivity(ActivityDTO activityDto){
            var activity = new Activity
            {
                Id = new Guid(),
                Action = activityDto.Action,
                Status = activityDto.Status,
                   Type = activityDto.Type,
                UserId = activityDto.UserId

            };

            var newActivity = await _repository.AddNewActivity(activity);
            return newActivity;
        }

        public async  Task<Activity> DeleteActivity(Activity activity) {
            var deletedActivity = await _repository.DeleteActivity(activity);
            return deletedActivity;
        }

        public async Task<Activity> DeleteActivity(string id)
        {
            var activityById = await _repository.GetActivityById(Guid.Parse(id));
            var deleteActivity = await _repository.DeleteActivity(activityById);
            return activityById;
        }

    }
}
