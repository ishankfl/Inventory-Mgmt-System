using System;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Mgmt_System.Models
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        // Navigation Property
        public User User { get; set; }

        // Parameterless constructor for EF
        public Category() { }

        // Constructor for convenience
        public Category(string name)
        {
            Name = name;
        }

        internal void setName(string name)
        {
            this.Name = name;
            throw new NotImplementedException();
        }

        internal void setUser(User user)
        {
            this.User = user;
            throw new NotImplementedException();
        }
    }
}
