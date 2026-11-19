using Microsoft.EntityFrameworkCore;
using ShopAKA.Models;

namespace ShopAKA.Data;

public class ShopDbContext : DbContext
{
    public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options) { }

    public DbSet<Category> Category => Set<Category>();
    public DbSet<Product> Product => Set<Product>();
    public DbSet<User> User => Set<User>();
    public DbSet<Customer> Customer => Set<Customer>();
    public DbSet<Order> Order => Set<Order>();
    public DbSet<OrderDetail> OrderDetail => Set<OrderDetail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().ToTable("Category");
        modelBuilder.Entity<Product>().ToTable("Product");
        modelBuilder.Entity<User>().ToTable("User");
        modelBuilder.Entity<Customer>().ToTable("Customer");
        modelBuilder.Entity<Order>().ToTable("Order");
        modelBuilder.Entity<OrderDetail>().ToTable("OrderDetail");

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Product)
            .HasForeignKey(p => p.CategoryID);

        modelBuilder.Entity<Customer>()
            .HasOne(c => c.User)
            .WithMany(u => u.Customer)
            .HasForeignKey(c => c.Username)
            .HasPrincipalKey(u => u.Username);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(c => c.Order)
            .HasForeignKey(o => o.CustomerID);

        modelBuilder.Entity<OrderDetail>()
            .HasOne(d => d.Order)
            .WithMany(o => o.OrderDetail)
            .HasForeignKey(d => d.OrderID);

        modelBuilder.Entity<OrderDetail>()
            .HasOne(d => d.Product)
            .WithMany(p => p.OrderDetail)
            .HasForeignKey(d => d.ProductID);
    }
}
