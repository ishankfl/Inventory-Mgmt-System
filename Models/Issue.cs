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

        public string IssueId { get; set; } // e.g., auto-generated "ISS-0001"

        [Required]
        public DateTime IssueDate { get; set; }

        [Required]
        [MaxLength(100)]
        public string Department { get; set; } // To whom issued

        [Required]
        [ForeignKey("IssuedByUser")]
        public Guid IssuedByUserId { get; set; }
        public User IssuedByUser { get; set; } // Reference to User model

        public ICollection<IssueDetail> IssueDetails { get; set; }
    }
}
