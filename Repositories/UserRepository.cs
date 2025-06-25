using Inventory_Mgmt_System.Data;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;


namespace Inventory_Mgmt_System.Repositories
{
    public class UserRepository:IUserRepository
    {
        private AppDbContext dbContext;
        private  DapperDbContext dapperDbContext;
        public UserRepository(AppDbContext _dbcontext, DapperDbContext dapperContext)
        {
            this.dbContext = _dbcontext;
            this.dapperDbContext = dapperContext; 
        }


        public async Task<List<User>> GetAllUser()
        {
            var users = new List<User>();

            using (var dbConnection = dapperDbContext.CreateConnection())
            {
                dbConnection.Open();
                var usersQuery = await dbConnection.QueryAsync<User>("SELECT * FROM \"Users\";");
                return usersQuery.ToList();
            }

        }

        public async Task<User> AddUserRepo(User user)
        {
            using (var dbConnection = dapperDbContext.CreateConnection())
            {
                dbConnection.Open();


                string insertQuery = @"
            INSERT INTO ""Users"" (""Id"", ""FullName"", ""Email"", ""PasswordHash"", ""CreatedAt"", ""Role"", ""PasswordSalt"")
            VALUES (@Id, @FullName, @Email, @PasswordHash, @CreatedAt, @Role, @PasswordSalt)
            RETURNING ""Id"";";

                // Make sure CreatedAt is set
                var createdAt = DateTime.UtcNow;

                var userId = await dbConnection.ExecuteScalarAsync<Guid>(insertQuery, new
                {
                    user.Id,
                    user.FullName,
                    user.Email, 
                    user.PasswordHash,
                    CreatedAt = createdAt,
                    user.Role,
                    user.PasswordSalt
                    
                });


                return user;
            }
        }


        // Method for get all users
        /*  public async Task<List<User>> GetAllUser()
          {
              var entry = await dbContext.Users.ToListAsync();
              return entry; // Return user
          }*/

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
