using System;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Mgmt_System.Models
{
    public class Item
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Unit { get; set; }

        [Required]
        public int Price { get; set; } = 0;

        public ICollection<Stock> Stock { get; set; }

    }
}

