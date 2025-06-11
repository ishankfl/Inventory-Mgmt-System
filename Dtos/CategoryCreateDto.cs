namespace Inventory_Mgmt_System.Dtos
{
    public class CategoryCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Guid UserId { get; set; }
    }
}
