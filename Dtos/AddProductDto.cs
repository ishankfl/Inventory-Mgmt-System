using System;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Mgmt_System.Dtos
{
    public class AddProductDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        public Guid UserId { get; set; }  // This will be set from the authenticated user or client
    }
}
