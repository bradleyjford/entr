using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Entr.Data.EntityFramework.Generators;

[Generator]
public class EntrEntityIdValueConverterGenerator : IIncrementalGenerator
{
    private const string MarkerAttribute = "Entr.Domain.EntityIdAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<StructDeclarationSyntax> typeDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null)!;

        IncrementalValueProvider<(Compilation Compilation, ImmutableArray<StructDeclarationSyntax> Classes)> compilationAndTypeDeclarations
            = context.CompilationProvider.Combine(typeDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndTypeDeclarations,
            static (spc, source) => Execute(source.Compilation, source.Classes, spc));
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node) =>
        node is StructDeclarationSyntax { AttributeLists.Count: > 0 };

    private static StructDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var declarationSyntax = (StructDeclarationSyntax)context.Node;

        foreach (var attributeList in declarationSyntax.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol attributeSymbol)
                {
                    // have we added the necessary references etc?
                    continue;
                }

                var containingType = attributeSymbol.ContainingType;
                var typeName = containingType.ToDisplayString();

                if (typeName.StartsWith(MarkerAttribute + "<"))
                {
                    return declarationSyntax;
                }
            }
        }

        return null;
    }

    private static void Execute(
        Compilation compilation,
        ImmutableArray<StructDeclarationSyntax> declarations,
        SourceProductionContext context)
    {
        if (declarations.IsDefaultOrEmpty)
        {
            // nothing to do yet
            return;
        }

        var typesToGenerate = GetTypesToGenerate(compilation, declarations, context.CancellationToken);

        if (typesToGenerate.Any())
        {
            var source = SourceCodeGenerator.GenerateSource(typesToGenerate);

            context.AddSource("EntrEntityFrameworkValueConverters.g.cs", SourceText.From(source, Encoding.UTF8));
        }
    }

    private static ImmutableArray<EntityIdInfo> GetTypesToGenerate(
        Compilation compilation,
        ImmutableArray<StructDeclarationSyntax> declarations,
        CancellationToken cancellationToken)
    {
        var result = ImmutableArray.CreateBuilder<EntityIdInfo>();

        foreach (var declaration in declarations)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var semanticModel = compilation.GetSemanticModel(declaration.SyntaxTree);

            if (semanticModel.GetDeclaredSymbol(declaration, cancellationToken) is not INamedTypeSymbol symbol)
            {
                // Have we referenced all the necessary assemblies.
                continue;
            }

            string? wrappedTypeName = null;

            var attributes = symbol.GetAttributes();

            foreach (var attribute in attributes)
            {
                // TODO: Support the generic interface
                //if (attribute.AttributeClass!.Name != MarkerAttribute)
                //{
                //    break;
                //}

                wrappedTypeName = attribute.AttributeClass!.TypeArguments.Single().ToDisplayString();

                break;
            }

            if (wrappedTypeName is null)
            {
                // create a diagnostic!
                continue;
            }

            var typeName = symbol.Name;
            var typeNamespace = symbol.ContainingNamespace.ToString();

            result.Add(new EntityIdInfo(typeNamespace, typeName, wrappedTypeName));
        }

        return result.ToImmutable();
    }
}
