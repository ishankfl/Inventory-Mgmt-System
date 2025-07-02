namespace Inventory_Mgmt_System.Dtos
{

    public class ReceiptCreateDto
    {
        public DateTime ReceiptDate { get; set; }
        public string BillNo { get; set; }
        public Guid VendorId { get; set; }
        public List<ReceiptDetailDto> ReceiptDetails { get; set; }
    }

    public class ReceiptDetailDto
    {
        public Guid ItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
    }
}

