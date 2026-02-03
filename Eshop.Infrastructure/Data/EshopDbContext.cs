using Eshop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Infrastructure
{
    public class EshopDbContext : DbContext
    {
        public EshopDbContext(DbContextOptions<EshopDbContext> options) : base(options)
        {
        }

        // Ова ќе креира табела "Products"
        public DbSet<Product> Products { get; set; }
        
    // Admins for application management
    public DbSet<Eshop.Domain.Entities.Admin> Admins { get; set; }

    // Registered users
    public DbSet<Eshop.Domain.Entities.User> Users { get; set; }
        
    // Shopping carts
    public DbSet<Eshop.Domain.Entities.Cart> Carts { get; set; }
    public DbSet<Eshop.Domain.Entities.CartItem> CartItems { get; set; }
        
        // Ова ќе креира табела "Orders"
        public DbSet<Order> Orders { get; set; }
        
        // Ова ќе креира табела "OrderItems"
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure decimal columns have precision to avoid truncation warnings
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasPrecision(18, 2);
        }
    }
}
