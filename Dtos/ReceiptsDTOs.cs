using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Mgmt_System.Dtos
{
  
    public class ReceiptCreateDto
    {
        [Required]
        public DateTime ReceiptDate { get; set; }

        [Required]
        [StringLength(100)]
        public string BillNo { get; set; } = string.Empty;

        [Required]
        public Guid VendorId { get; set; }

        [StringLength(50)]
        public string? ReceiptId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one receipt detail is required")]
        public List<ReceiptDetailCreateDto> ReceiptDetails { get; set; } = new List<ReceiptDetailCreateDto>();
    }


    public class ReceiptDetailCreateDto
    {
        [Required]
        public Guid ItemId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be positive")]
        public decimal Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Rate cannot be negative")]
        public decimal Rate { get; set; }
    }

 
    public class ReceiptUpdateDto
    {
        [Required]
        public DateTime ReceiptDate { get; set; }

        [Required]
        [StringLength(100)]
        public string BillNo { get; set; } = string.Empty;

        [Required]
        public Guid VendorId { get; set; }

        public List<ReceiptDetailUpdateDto> ReceiptDetails { get; set; } = new List<ReceiptDetailUpdateDto>();
    }


    public class ReceiptDetailUpdateDto
    {
        public Guid? Id { get; set; }

        [Required]
        public Guid ItemId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be positive")]
        public decimal Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Rate cannot be negative")]
        public decimal Rate { get; set; }
    }
}