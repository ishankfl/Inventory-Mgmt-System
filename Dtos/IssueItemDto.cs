namespace Inventory_Mgmt_System.Dtos
{
    public class IssueItemDto
    {
        public Guid ProductId { get; set; }
        public int QuantityIssued { get; set; }
    }
}