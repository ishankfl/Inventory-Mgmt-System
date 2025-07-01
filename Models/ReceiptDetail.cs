using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory_Mgmt_System.Models
{
    public class ReceiptDetail
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [ForeignKey("Receipt")]
        public Guid ReceiptId { get; set; }
        public Receipt Receipt { get; set; }

        [Required]
        [ForeignKey("Item")]
        public Guid ItemId { get; set; }
        public Item Item { get; set; }

        [Required]
        public decimal Quantity { get; set; }

        [Required]
        public decimal Rate { get; set; }
    }
}
