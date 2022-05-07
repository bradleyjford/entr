using System.Text;
using Xunit.Abstractions;

namespace Entr.Domain.Generators.Tests;

[UsesVerify]
public class EntrEntityIdGeneratorTests
{
    private readonly ITestOutputHelper _output;

    public EntrEntityIdGeneratorTests(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact]
    public Task GeneratesEntityIdClassCorrectly()
    {
        // The source code to test
        var source = @"
using System;
using Entr.Domain;

namespace Entr.Domain.Generators.Tests
{
    [EntityId<Guid>]
    public struct UserId {}

    [EntityId<int>]
    public struct RoleId {}
}
";

        // Pass the source code to our helper and snapshot test the output
        return TestHelper.Verify(source);
    }

    [Fact]
    public void PeekAtSource()
    {
        var idInfo = new EntityIdInfo("SampleApi.Products", "ProductId", "Guid");
        var builder = new StringBuilder();

        SourceCodeGenerator.GenerateEntityIdType(idInfo, builder);

        _output.WriteLine(builder.ToString());
    }
}
