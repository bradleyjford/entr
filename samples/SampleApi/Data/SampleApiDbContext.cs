using SampleApi.Products;

namespace SampleApi.Data;

public class SampleApiDbContext : DbContext
{
    public SampleApiDbContext(DbContextOptions<SampleApiDbContext> options)
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
