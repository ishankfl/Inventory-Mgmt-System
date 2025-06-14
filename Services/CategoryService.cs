using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Inventory_Mgmt_System.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Mgmt_System.Services
{
    public class CategoryService : ICategoryService
    {
        private ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository) {
            this._categoryRepository = categoryRepository;
        }

        public async Task<Category> CreateCategory(Category category)
        {
            var updated = await _categoryRepository.CreateCategory(category);
            return updated;
        }

        public async Task<Category> GetCategoryById(Guid id)
        {
            var category = await _categoryRepository.GetCategoryById(id);
            return category;
        }

        public async Task<List<Category>> GetAllCategories()
        {
            var category = await _categoryRepository.GetAllCategories();
            return category;
        }

        public async Task<List<Category>> GetCategoryByUser(Guid id)
        {
            var category = await _categoryRepository.GetCategoryByUser(id);
            return category;
        }

        public async Task<Category> UpdateCategory(Category updatedCategory)
        {
            var existingCategory = await _categoryRepository.UpdateCategory(updatedCategory);
            return existingCategory;
        }
        public async Task<Category> DeleteCategory(Guid id)
        {
            var category = await  _categoryRepository.DeleteCategory(id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }

            return category;
        }

    }
}
