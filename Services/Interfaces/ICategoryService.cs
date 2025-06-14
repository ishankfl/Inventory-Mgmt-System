using Inventory_Mgmt_System.Models;

namespace Inventory_Mgmt_System.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<Category> CreateCategory(Category category);
        Task<Category> GetCategoryById(Guid id);
        Task<List<Category>> GetAllCategories();

        Task<List<Category>> GetCategoryByUser(Guid id);
        Task<Category> UpdateCategory(Category updatedCategory);
        Task<Category> DeleteCategory(Guid id);

        Task<Category> GetCategoryByName(string name);
    }
}
