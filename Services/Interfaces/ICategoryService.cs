using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;

namespace Inventory_Mgmt_System.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<Category> CreateCategory(CategoryCreateDto categorydto, Guid performedByUserId);
        Task<Category> GetCategoryById(Guid id);
        Task<Category> GetCategoryByName(string name);
        Task<List<Category>> GetAllCategories();
        Task<List<Category>> GetCategoryByUser(Guid id);
        Task<Category> UpdateCategory(Guid id, CategoryCreateDto categoryDto, Guid performedByUserId);
        Task<Category> DeleteCategory(Guid id, Guid performedByUserId);
        Task<(List<Category> categories, int totalCount)> GetAllCategoriesByPaginationFilter(int page = 1, int pageSize = 6, string? search = null);
    }
}
