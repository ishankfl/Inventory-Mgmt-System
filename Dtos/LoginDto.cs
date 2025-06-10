using System.ComponentModel.DataAnnotations;

namespace Inventory_Mgmt_System.Dtos
{
    public class LoginDto
    {
        [EmailAddress]
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? password { get; set; }
    }
}
