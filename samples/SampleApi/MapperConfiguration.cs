using AutoMapper;
using SampleApi.Products;

namespace SampleApi;

public static class AutomapperConfiguration
{
    public static void Configure(IMapperConfigurationExpression config)
    {
        config.AddProfile<ProductsMappingProfile>();
    }
}
