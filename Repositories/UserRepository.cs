using Inventory_Mgmt_System.Data;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Inventory_Mgmt_System.Repositories
{
    public class UserRepository:IUserRepository
    {
        private AppDbContext dbContext;
        public UserRepository(AppDbContext _dbcontext)
        {
            this.dbContext = _dbcontext;
        }

        // Method for add new user or Register
        public async Task<User> AddUserRepo(User user)
        {
            var entry = await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();
            return entry.Entity; // Return user
        }
        // Method for get all users
        public async Task<List<User>> GetAllUser()
        {
            var entry = await dbContext.Users.ToListAsync();
            return entry; // Return user
        }

        public async Task<User> CheckEmailAndPassword(string email, string password)
        {
            var user =await  dbContext.Users.FirstOrDefaultAsync(data => data.Email==email && data.PasswordHash==password);
            
            return user;
        }

       public async Task<User> GetUserByEmailAsync(string Email)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(data=>data.Email==Email);
            return user;
        }

        public async Task<User> DeleteUserById(Guid id)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Id==id);
            if (user == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }
            dbContext.Users.Remove(user);
            await dbContext.SaveChangesAsync();
            return user;
        }

    }
}
