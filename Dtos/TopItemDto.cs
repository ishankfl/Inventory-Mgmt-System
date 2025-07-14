namespace Inventory_Mgmt_System.Dtos
{
    public class TopItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public int TotalStockQuantity { get; set; }
    }

    public class TopIssuedItemResponseDto
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; }
        public decimal TotalIssuedQuantity { get; set; }
    }
}
