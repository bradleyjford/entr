using SampleApi.Products;
using SampleApi.Security;

namespace SampleApi.Data;

public class SampleApiDbContext : DbContext
{
    public SampleApiDbContext(DbContextOptions<SampleApiDbContext> options)
        : base(options)
    { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<User> Users => Set<User>();

#if DEBUG
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableDetailedErrors();
        optionsBuilder.EnableSensitiveDataLogging();
    }
#endif

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var productEntity = modelBuilder.Entity<Product>();

        productEntity
            .ToTable("Product")
            .HasKey(p => p.Id);

        productEntity.Property(p => p.Name)
            .HasConversion<string>(
                pn => pn.Value,
                v => new ProductName(v));

        productEntity.Property("_rowVersion")
            .HasColumnName("RowVersion")
            .IsRowVersion();

        var userEntity = modelBuilder.Entity<User>();

        userEntity
            .ToTable("User")
            .HasKey(p => p.Id);

        userEntity.Property(p => p.Email)
            .HasConversion<string>(
                e => e.Value,
                v => new EmailAddress(v));

        userEntity.Property("_rowVersion")
            .IsConcurrencyToken();

        userEntity.HasMany(u => u.Roles);
        userEntity.Navigation(e => e.Roles)
            .AutoInclude();
    }
}
