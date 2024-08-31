using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIntegration.DbContext;

public class ProductDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    
    public DbSet<Order> Orders { get; set; }
    
    public DbSet<OrderDetail> OrderDetails { get; set; }
    
    public DbSet<Product> Products { get; set; }
}