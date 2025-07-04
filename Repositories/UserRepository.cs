using Inventory_Mgmt_System.Data;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

using Dapper;



namespace Inventory_Mgmt_System.Repositories
{
    public class UserRepository:IUserRepository
    {
        private  DapperDbContext dapperDbContext;
        public UserRepository( DapperDbContext dapperContext)
        {
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
           // var user =await  dbContext.Users.FirstOrDefaultAsync(data => data.Email==email && data.PasswordHash==password);
           using (var dbConnection = dapperDbContext.CreateConnection())
            {
                dbConnection.Open();

                string query = @"SELECT * FROM ""Users"" WHERE Email = @Email and PasswordHash = @PasswordHash";
                var user = await dbConnection.ExecuteScalarAsync<User>(query, new
                {
                    Email = email,
                    PasswordHash = password
                });
                 return user;
            }
            
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            using (var dbConnection = dapperDbContext.CreateConnection())
            {
                dbConnection.Open();
                string query = @"SELECT * FROM ""Users"" WHERE ""Email"" = @Email";

                var user = await dbConnection.QueryFirstOrDefaultAsync<User>(query, new { Email = email });
                    return user;
            }
        }


        public async Task<User> DeleteUserById(Guid id)
        {
            using (var dbConnection = dapperDbContext.CreateConnection())
            {
                dbConnection.Open();

                string selectQuery = @"SELECT * FROM ""Users"" WHERE ""Id"" = @Id";
                var user = await dbConnection.QueryFirstOrDefaultAsync<User>(selectQuery, new { Id = id });

                if (user == null)
                {
                    throw new KeyNotFoundException($"User with ID {id} not found.");
                }

                string deleteQuery = @"DELETE FROM ""Users"" WHERE ""Id"" = @Id";
                await dbConnection.ExecuteAsync(deleteQuery, new { Id = id });

                return user;
            }
        }


        public async Task<int> TotalNumberOfUser()
        {
            using (var dbConnection = dapperDbContext.CreateConnection())
            {
                dbConnection.Open();

                string query = @"SELECT COUNT(*) FROM ""Users""";
                var count = await dbConnection.ExecuteScalarAsync<int>(query);

                return count;
            }
        }

        public async Task<User> GetUserById(Guid id)
        {
            using (var dbConnection = dapperDbContext.CreateConnection())
            {
                dbConnection.Open();

                string query = @"SELECT * FROM ""Users"" WHERE ""Id"" = @Id";
                var user = await dbConnection.QueryFirstOrDefaultAsync<User>(query, new { Id = id });

                return user;
            }
        }



    }
}
