using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory_Mgmt_System.Models
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public Category() { }

        public Category(string name)
        {
            Name = name;
        }

        internal void setName(string name)
        {
            this.Name = name;
        }

        internal void setUser(User user)
        {
            this.User = user;
        }
    }
}
