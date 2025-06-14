using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory_Mgmt_System.Models
{
    public class IssueItem
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ProductIssueId { get; set; }
        public virtual ProductIssue ProductIssue { get; set; }

        [Required]
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }

        [Required]
        public int QuantityIssued { get; set; }
    }
}
