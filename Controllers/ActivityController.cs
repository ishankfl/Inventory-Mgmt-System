using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using NuGet.Protocol;

namespace Inventory_Mgmt_System.Controllers
{
    [Route("api/[controller]")]
        [ApiController]
        [Authorize]

    public class ActivityController:ControllerBase


    {
        private readonly IActivityServices _service;
        public ActivityController(IActivityServices service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllActivities()
        {
            try
            {

                var allActivities = await _service.GetAllActivities();
                return Ok(allActivities);
            }
            catch (Exception ex) { 
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateActivity( ActivityDTO activityDto)
        {
            try
            {
                var activity = new Activity
                {
                    Id = new Guid(),
                    Action = activityDto.Action,
                    Status = activityDto.Status,
                    /*Timestamp  =activityDto.Timestamp,*/
                    Type = activityDto.Type,
                    UserId = activityDto.UserId

                };

                var activityAdded = await _service.AddNewActivity(activity);
                return StatusCode(201,activityAdded);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(string id)
    {
            try
            {

                var result = await _service.DeleteActivity(id);
                return Ok(result);
            }
            catch (Exception ex) { 
                return StatusCode(500,ex.Message);
            }

    }
    }

}
