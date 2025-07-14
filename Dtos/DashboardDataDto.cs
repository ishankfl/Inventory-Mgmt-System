namespace Inventory_Mgmt_System.Dtos
{
    public class DashboardDataDto
    {
        public List<TopItemDto> TopProductsByQty { get; set; }
        public List<TopIssuedItemResponseDto> TopIssuedItems { get; set; }
        public Dictionary<string, dynamic> CardCounts { get; set; }
        public List<DailyTotalPriceDto> DailyTotalReceiptValue { get; set; }
    }
}
