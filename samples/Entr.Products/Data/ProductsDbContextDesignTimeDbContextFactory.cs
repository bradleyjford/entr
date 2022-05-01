using Entr.Data.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;

namespace Entr.Products.Data;

public class ProductsDbContextDesignTimeDbContextFactory : IDesignTimeDbContextFactory<ProductsDbContext>
{
    public ProductsDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<ProductsDbContext>();
        
        builder.UseSqlServer(args[1]);
        builder.ReplaceService<IValueConverterSelector, EntrEntityIdValueConverterSelector>();

        return new ProductsDbContext(builder.Options);
    }
}
