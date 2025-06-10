using System.ComponentModel.DataAnnotations;

namespace Inventory_Mgmt_System.Models
{
    // Defines user roles in the system
    public enum UserRole
    {
        Admin,
        Staff
    }
    // Represents a user model in the system
    public class User
    {
        public Guid Id { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }

        [Required]
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
