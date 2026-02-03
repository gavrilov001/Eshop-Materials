using System;
using System.Collections.Generic;

namespace Eshop.Domain.Entities
{
    public class Cart
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        // Navigation
        public User User { get; set; } = null!;

        public List<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
