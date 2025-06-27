using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory_Mgmt_System.Models
{
    public class Activity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public ActivityType Type { get; set; }

        [Required]
        public string Action { get; set; }  

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string Status { get; set; }  

        [ForeignKey(nameof(User))]
        public Guid? UserId { get; set; }   

        public virtual User User { get; set; }
    }

    public enum ActivityType
    {
        ProductAdded,
        ProductRemoved,
        ProductUpdated,
        ProductDeleted,
        ProductIssued,
        UserLoggedIn,
        UserLoggedOut,
        UserAdded,
        LowStockAlert,
        DocumentUploaded,
        InventoryChecked
    }
}
