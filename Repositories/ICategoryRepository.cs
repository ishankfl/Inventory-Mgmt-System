using Inventory_Mgmt_System.Models;

namespace Inventory_Mgmt_System.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category> GetCategoryById(Guid id);
        Task<Category> CreateCategory(Category category);

    }
}
