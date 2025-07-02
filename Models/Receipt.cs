using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory_Mgmt_System.Models
{
    public class Receipt
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string ReceiptId { get; set; }

        [Required]
        public DateTime ReceiptDate { get; set; }

        [Required]
        [MaxLength(100)]
        public string BillNo { get; set; }

        [Required]
        [ForeignKey("Vendor")]
        public Guid VendorId { get; set; }
        public Vendor Vendor { get; set; }

        public ICollection<ReceiptDetail> ReceiptDetails { get; set; }
    }
}
