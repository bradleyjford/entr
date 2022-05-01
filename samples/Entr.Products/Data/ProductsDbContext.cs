using Entr.Products.Domain;
using Microsoft.EntityFrameworkCore;

namespace Entr.Products.Data;

public class ProductsDbContext : DbContext
{
    public ProductsDbContext(DbContextOptions<ProductsDbContext> options) 
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .ToTable("Product")
            .HasKey(p => p.Id);
    }
}
