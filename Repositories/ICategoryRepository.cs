using Inventory_Mgmt_System.Models;

namespace Inventory_Mgmt_System.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category> GetCategoryById(Guid id);
        Task<Category> CreateCategory(Category category);

        Task<Category> UpdateCategory(Category updatedCategory);
        Task<List<Category>> GetCategoryByUser(Guid id);
        Task<List<Category>> GetAllCategories();
      /*  Task<Category> GetCategoryById(Guid id);
        Task<Category> CreateCategory(Category category)*/

    }
}
