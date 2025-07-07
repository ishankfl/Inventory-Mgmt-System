using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Inventory_Mgmt_System.Services.Interfaces;

namespace Inventory_Mgmt_System.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Category> CreateCategory(CategoryCreateDto categorydto)
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = categorydto.Name,
                UserId = categorydto.UserId,
                Description = categorydto.Description,
            };

            var isExistCategory = await GetCategoryByName(categorydto.Name);
            if (isExistCategory != null)
            {
                throw new InvalidOperationException($"Category with name '{categorydto.Name}' already exists.");
            }

            var created = await _categoryRepository.CreateCategory(category);
            return created;
        }

        public async Task<Category> GetCategoryById(Guid id)
        {
            return await _categoryRepository.GetCategoryById(id);
        }

        public async Task<Category> GetCategoryByName(string name)
        {
            return await _categoryRepository.GetCategoryByName(name);
        }

        public async Task<List<Category>> GetAllCategories()
        {
            return await _categoryRepository.GetAllCategories();
        }

        public async Task<List<Category>> GetCategoryByUser(Guid id)
        {
            return await _categoryRepository.GetCategoryByUser(id);
        }

        public async Task<Category> UpdateCategory(Guid id, CategoryCreateDto categoryDto)
        {
            var existingCategory = await _categoryRepository.GetCategoryById(id);
            if (existingCategory == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }

            var categoryByName = await _categoryRepository.GetCategoryByName(categoryDto.Name);
            if (categoryByName != null && categoryByName.Id != existingCategory.Id)
            {
                throw new InvalidOperationException($"Category with name '{categoryDto.Name}' already exists.");
            }

            existingCategory.setName(categoryDto.Name);
            existingCategory.Description = categoryDto.Description;

            var updatedCategory = await _categoryRepository.UpdateCategory(existingCategory);
            return updatedCategory;
        }


        public async Task<Category> DeleteCategory(Guid id)
        {
            var category = await _categoryRepository.DeleteCategory(id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }

            return category;
        }
    }
}
