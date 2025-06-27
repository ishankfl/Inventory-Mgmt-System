using Inventory_Mgmt_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Mgmt_System.Dtos
{
    public class ActivityDTO
    {

        [Required]
        public ActivityType Type { get; set; }

        [Required]
        [StringLength(250, ErrorMessage = "Action description cannot exceed 250 characters.")]
        public string Action { get; set; }

        public string Status { get; set; } 

        public Guid? UserId { get; set; }  



    }
}
