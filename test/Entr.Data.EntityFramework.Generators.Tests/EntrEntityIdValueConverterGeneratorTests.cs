using System.Collections.Immutable;
using Xunit.Abstractions;

namespace Entr.Data.EntityFramework.Generators.Tests;

[UsesVerify]
public class EntrEntityIdValueConverterGeneratorTests
{
    private readonly ITestOutputHelper _output;

    public EntrEntityIdValueConverterGeneratorTests(ITestOutputHelper output)
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

namespace Entr.Data.EntityFramework.Generators.Tests
{
    [EntityId<int>]
    public struct UserId {}

    [EntityId<Guid>]
    public struct RoleId {}
}
";

        // Pass the source code to our helper and snapshot test the output
        return TestHelper.Verify(source);
    }
    
    [Fact]
    public void PeekAtSource()
    {
        var items = ImmutableArray.CreateBuilder<EntityIdInfo>();
        items.Add(new EntityIdInfo("Entr.Domain.Generators.Tests", "ProductId", "Guid"));
        
        var source = SourceCodeGenerator.GenerateSource(items.ToImmutable());

        _output.WriteLine(source);
    }
}
