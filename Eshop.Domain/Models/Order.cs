using System;
using System.Collections.Generic;

namespace Eshop.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        
        public string CustomerName { get; set; } = null!;
        public string CustomerSurname { get; set; } = null!;
        public string CustomerAddress { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;

        public DateTime DateCreated { get; set; } = DateTime.Now;
        
        public decimal TotalPrice { get; set; }

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
