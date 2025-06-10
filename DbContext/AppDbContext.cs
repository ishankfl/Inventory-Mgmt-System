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

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // One User can create multiple Categories, one Category belongs to one User
            modelBuilder.Entity<Category>()
                .HasOne(category => category.User)
                .WithMany()               // You can specify collection property if User has e.g. ICollection<Category> Categories
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);  // Deleting User will delete related Categories

            // One User can create multiple Products, one Product belongs to one User
            modelBuilder.Entity<Product>()
                .HasOne(product => product.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);  // Deleting User will delete related Products

            // One Category can have multiple Products, one Product belongs to one Category
            modelBuilder.Entity<Product>()
                .HasOne(product => product.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);  // Deleting Category will delete related Products

            base.OnModelCreating(modelBuilder);
        }




    }
}
