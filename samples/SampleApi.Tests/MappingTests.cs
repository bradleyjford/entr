using AutoMapper;
using Xunit;

namespace SampleApi.Tests;

public class MappingTests
{
    [Fact]
    public void MappingConfigurationIsValid()
    {
        var config = new MapperConfigurationExpression();

        AutomapperConfiguration.Configure(config);

        var mapperConfig = new MapperConfiguration(config);

        mapperConfig.AssertConfigurationIsValid();
    }
}
