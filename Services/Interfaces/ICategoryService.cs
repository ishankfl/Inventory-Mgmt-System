using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;

namespace Inventory_Mgmt_System.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<Category> CreateCategory(CategoryCreateDto category);
        Task<Category> GetCategoryById(Guid id);
        Task<List<Category>> GetAllCategories();

        Task<List<Category>> GetCategoryByUser(Guid id);
        Task<Category> UpdateCategory(Guid id, CategoryCreateDto categoryDto);
        Task<Category> DeleteCategory(Guid id);

        Task<Category> GetCategoryByName(string name);
    }
}
