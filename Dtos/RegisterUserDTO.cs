using Inventory_Mgmt_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Mgmt_System.Dtos
{
    public class RegisterUserDTO
    {
        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }  // Plaintext password input

        [Required]
        public UserRole Role { get; set; }
    }

}
