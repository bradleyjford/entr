using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Entr.Domain.Generators.Tests;

public static class TestHelper
{
    public static Task Verify(string source)
    {
        // Parse the provided string into a C# syntax tree
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        
        // Create references for assemblies we require
        // We could add multiple references if required
        IEnumerable<PortableExecutableReference> references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Attribute).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(JsonConverter).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(SequentialGuidGenerator).Assembly.Location),
        };

        var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

        // Create a Roslyn compilation for the syntax tree.
        var compilation = CSharpCompilation.Create(
            assemblyName: "DomainGeneratorTests",
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: options);

        // Create an instance of our EnumGenerator incremental source generator
        var generator = new EntrEntityIdGenerator();

        // The GeneratorDriver is used to run our generator against a compilation
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Run the source generator!
        driver = driver.RunGenerators(compilation);

        var diagnostics = compilation.GetDiagnostics();

        // Use verify to snapshot test the source generator output!
        return Verifier.Verify(driver)
            .UseDirectory("Snapshots");
    }
}

