using Inventory_Mgmt_System.Data;
using Inventory_Mgmt_System.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Mgmt_System.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private AppDbContext _context;
        public CategoryRepository(AppDbContext context) { 
            this._context = context;
        }
        public async  Task<Category> CreateCategory(Category category)
        {
            var updated = await  _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return updated.Entity;
        }

        public async Task<Category> GetCategoryById(Guid id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(caty =>caty.Id == id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }
            return category;
        }

        public async Task<List<Category>> GetAllCategories()
        {
            var category = await _context.Categories.ToListAsync();
            if (category == null)
            {
                return [];
            }
            return category;
        }

        public async Task<List<Category>> GetCategoryByUser(Guid id)
        {
            var category = await _context.Categories.Where(category=> category.User.Id == id).ToListAsync();
            if (category == null)
            {
                return [];
            }
            return category;
        }

        public async Task<Category> UpdateCategory(Category updatedCategory)
        {
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == updatedCategory.Id);

            if (existingCategory == null)
            {
                throw new KeyNotFoundException($"Category with ID {updatedCategory.Id} not found.");
            }

            // Update properties
            existingCategory.setName(updatedCategory.Name); // Using setter method 
            existingCategory.setUser(updatedCategory.User); // Using setter method 

            // Save changes
            await _context.SaveChangesAsync();  

            return existingCategory;
        }
        public async Task<Category> DeleteCategory(Guid id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(caty => caty.Id == id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return category;
        }

    }
}
