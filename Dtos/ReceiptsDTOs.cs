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

    public class ReceiptUpdateDto
    {
        public DateTime ReceiptDate { get; set; }
        public string BillNo { get; set; }
        public Guid VendorId { get; set; }
        public List<ReceiptDetailUpdateDto> ReceiptDetails { get; set; }
    }

    public class ReceiptDetailUpdateDto
    {
        public Guid? Id { get; set; }  
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Rate { get; set; }
    }
}

