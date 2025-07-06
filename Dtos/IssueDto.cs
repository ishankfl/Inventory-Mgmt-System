using Inventory_Mgmt_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Mgmt_System.Dtos
{
    public class IssueDto
    {
        public string IssueId { get; set; }

        [Required]
        public DateTime IssueDate { get; set; }

        [MaxLength(100)]
        public string InvoiceNumber { get; set; }

        public DateTime? InvoiceDate { get; set; }

        [MaxLength(500)]
        public string DeliveryNote { get; set; }

        [Required]
        public Guid IssuedByUserId { get; set; }

        [Required]
        public Guid DepartmentId { get; set; }

        [Required]
        public List<IssueDetailDto> IssueDetails { get; set; }
    }

    public class IssueDetailDto
    {
        [Required]
        public Guid ItemId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public decimal Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Rate cannot be negative.")]
        public decimal IssueRate { get; set; }
    }
}
