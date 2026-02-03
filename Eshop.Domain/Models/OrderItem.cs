using System;

namespace Eshop.Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public Product Product { get; set; } = null!;
        public Guid ProductId { get; set; }
        public int Quantity { get; set; } 
        public decimal Price { get; set; } 
    }
}
