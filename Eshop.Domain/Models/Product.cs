using System;
using System.ComponentModel.DataAnnotations;

namespace Eshop.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        public string ImageUrl { get; set; } = null!;
        public string Unit { get; set; } = "парче";

        public string Category { get; set; } = null!;
        
        public bool IsInStock { get; set; } 
    }
}
