using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Mgmt_System.Dtos
{
    public class IssueRequestDto
    {
        [Required]
        public Guid DepartmentId { get; set; }

        [Required]
        public Guid IssuedById { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one item is required.")]
        public List<IssueItemDto> Items { get; set; }
    }


    public class IssueRequestDtoOne
    {
        [Required]
        public Guid DepartmentId { get; set; }

        [Required]
        public Guid IssuedById { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one item is required.")]
        public IssueItemDto Item { get; set; }
    }
}