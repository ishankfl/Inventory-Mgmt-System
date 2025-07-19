using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Inventory_Mgmt_System.Services.Interfaces;

namespace Inventory_Mgmt_System.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IActivityServices _activityServices;

        public CategoryService(ICategoryRepository categoryRepository, IActivityServices activityServices)
        {
            _categoryRepository = categoryRepository;
            _activityServices = activityServices;
        }

        public async Task<Category> CreateCategory(CategoryCreateDto categorydto, Guid performedByUserId)
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = categorydto.Name,
                UserId = performedByUserId,
                Description = categorydto.Description,
            };

            var isExistCategory = await GetCategoryByName(categorydto.Name);
            if (isExistCategory != null)
            {
                throw new InvalidOperationException($"Category with name '{categorydto.Name}' already exists.");
            }

            var created = await _categoryRepository.CreateCategory(category);

            if (created != null)
            {
                var activity = new ActivityDTO
                {
                    Action = $"Category created: {categorydto.Name}",
                    Status = "success",
                    Type = ActivityType.CategoryCreated,
                    UserId = performedByUserId
                };
                await _activityServices.AddNewActivity(activity);
            }

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

        public async Task<(List<Category> categories, int totalCount)> GetAllCategoriesByPaginationFilter(int page = 1, int pageSize = 6, string? search = null)
        {
            return await _categoryRepository.GetAllCategoriesByPaginationFilter(page,pageSize,search);
        }
        public async Task<List<Category>> GetCategoryByUser(Guid id)
        {
            return await _categoryRepository.GetCategoryByUser(id);
        }

        public async Task<Category> UpdateCategory(Guid id, CategoryCreateDto categoryDto, Guid performedByUserId)
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

            string oldName = existingCategory.Name;
            existingCategory.setName(categoryDto.Name);
            existingCategory.Description = categoryDto.Description;

            var updatedCategory = await _categoryRepository.UpdateCategory(existingCategory);

            if (updatedCategory != null)
            {
                var activity = new ActivityDTO
                {
                    Action = $"Category updated from '{oldName}' to '{categoryDto.Name}'",
                    Status = "info",
                    Type = ActivityType.CategoryUpdated,
                    UserId = performedByUserId
                };
                await _activityServices.AddNewActivity(activity);
            }

            return updatedCategory;
        }

        public async Task<Category> DeleteCategory(Guid id, Guid performedByUserId)
        {
            var category = await _categoryRepository.GetCategoryById(id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }

            var deletedCategory = await _categoryRepository.DeleteCategory(id);

            if (deletedCategory != null)
            {
                var activity = new ActivityDTO
                {
                    Action = $"Category deleted: {category.Name}",
                    Status = "danger",
                    Type = ActivityType.CategoryDeleted,
                    UserId = performedByUserId
                };
                await _activityServices.AddNewActivity(activity);
            }

            return deletedCategory;
        }
    }
}