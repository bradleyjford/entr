using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SampleApi.Data;

public class SampleApiDbContextDesignTimeDbContextFactory : IDesignTimeDbContextFactory<SampleApiDbContext>
{
    public SampleApiDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<SampleApiDbContext>();

        builder.UseSqlServer(args[0]);
        builder.ReplaceService<IValueConverterSelector, EntrEntityIdValueConverterSelector>();

        return new SampleApiDbContext(builder.Options);
    }
}
