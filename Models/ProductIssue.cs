using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory_Mgmt_System.Models
{
    public class ProductIssue
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        [Required]
        public Guid IssuedById { get; set; } // Staff who issued
        public virtual User IssuedBy { get; set; }

        public DateTime IssueDate { get; set; } = DateTime.UtcNow;

        public bool IsCompleted { get; set; } = false;

        public virtual ICollection<IssueItem> IssueItems { get; set; } = new List<IssueItem>();
    }
}
