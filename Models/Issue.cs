using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory_Mgmt_System.Models
{
    public class Issue
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string IssueId { get; set; }

        [Required]
        public DateTime IssueDate { get; set; }

        [MaxLength(100)]
        public string InvoiceNumber { get; set; }

        public DateTime? InvoiceDate { get; set; }

        [MaxLength(255)]
        public string Customer { get; set; }

        [MaxLength(500)]
        public string DeliveryNote { get; set; }

        [Required]
        [MaxLength(100)]
        [ForeignKey("IssuedByUser")]
        public Guid IssuedByUserId { get; set; }
        public User IssuedByUser { get; set; }

        [Required]
        [MaxLength(100)]
        public string Department { get; set; }

        public ICollection<IssueDetail> IssueDetails { get; set; }
    }
}
