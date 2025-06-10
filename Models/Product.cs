using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory_Mgmt_System.Models
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Foreign key to Category
        [ForeignKey("Category")]
        public Guid CategoryId { get; set; }

        public virtual Category Category { get; set; }

        // Optional: track who created the product
        [ForeignKey("User")]
        public Guid UserId { get; set; }

        public virtual User User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Default constructor
        public Product() { }

        // Constructor with required fields
        public Product(string name, int quantity, decimal price, Guid categoryId)
        {
            Name = name;
            Quantity = quantity;
            Price = price;
            CategoryId = categoryId;
        }

        // Internal setter methods
        internal void setName(string name)
        {
            this.Name = name;
        }

        internal void setQuantity(int quantity)
        {
            this.Quantity = quantity;
        }

        internal void setPrice(decimal price)
        {
            this.Price = price;
        }

        internal void setCategory(Category category)
        {
            this.Category = category;
        }

        internal void setUser(User user)
        {
            this.User = user;
        }
    }
}
