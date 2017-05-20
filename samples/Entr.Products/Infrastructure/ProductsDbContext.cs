using Entr.Data;
using Entr.Products.Domain;
using Microsoft.EntityFrameworkCore;


namespace Entr.Products
{
    public class ProductsDbContext : DbContext
    {
        public ProductsDbContext(DbContextOptions<ProductsDbContext> options) 
            : base(options)
        {
        }

        public DbSet<Product> Products { get; protected set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().ToTable("Product");
        }
    }
}
