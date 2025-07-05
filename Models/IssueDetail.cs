using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory_Mgmt_System.Models
{
    public class IssueDetail
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [ForeignKey("Issue")]
        public Guid IssueId { get; set; }
        public Issue Issue { get; set; }

        [Required]
        [ForeignKey("Item")]
        public Guid ItemId { get; set; }
        public Item Item { get; set; }

        public decimal? IssueRate { get; set; }

        [Required]
        public decimal Quantity { get; set; }
    }
}
