using Inventory_Mgmt_System.Models;           // Importing model classes (e.g., User)
using Microsoft.EntityFrameworkCore;          // Importing EF Core features like DbContext and DbSet

namespace Inventory_Mgmt_System.Data 
{    
    /// AppDbContext represents the session with the database and provides APIs to interact with the data.
    public class AppDbContext : DbContext
    {
        /// Constructor that passes DbContext options to the base class.
        /// These options typically include the connection string and provider (e.g., PostgreSQL).
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// Represents the Users table in the database.
        /// Allows CRUD operations on User entities.
        public DbSet<User> Users { get; set; }

        public DbSet<Category> Categories { get; set; }



    }
}
