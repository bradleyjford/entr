using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Entr.Domain.Generators;

[Generator]
public class EntrEntityIdGenerator : IIncrementalGenerator
{
    private const string MarkerAttribute = "Entr.Domain.EntityIdAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "EntityIdAttribute.g.cs", SourceText.From(SourceCodeGenerator.Attribute, Encoding.UTF8)));
        
        IncrementalValuesProvider<StructDeclarationSyntax> classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null)!;

        IncrementalValueProvider<(Compilation Compilation, ImmutableArray<StructDeclarationSyntax> Classes)> compilationsAndTypeDeclarations
            = context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationsAndTypeDeclarations,
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
        ImmutableArray<StructDeclarationSyntax> typeSyntaxes, 
        SourceProductionContext context)
    {
        if (typeSyntaxes.IsDefaultOrEmpty)
        {
            // nothing to do yet
            return;
        }

        //var distinctClasses = typeSyntaxes.Distinct();

        var typesToGenerate = GetTypesToGenerate(compilation, typeSyntaxes, context.CancellationToken);

        if (typesToGenerate.Any())
        {
            var source = SourceCodeGenerator.GenerateSource(typesToGenerate);

            context.AddSource("EntrEntityIds.g.cs", SourceText.From(source, Encoding.UTF8));
        }
    }

    private static ImmutableArray<EntityIdInfo> GetTypesToGenerate(
        Compilation compilation, 
        ImmutableArray<StructDeclarationSyntax> declarationSyntaxes, 
        CancellationToken cancellationToken)
    {
        var typesToGenerate = ImmutableArray.CreateBuilder<EntityIdInfo>();

        //var markerAttributeSymbol = compilation.GetTypeByMetadataName(MarkerAttribute);

        //if (markerAttributeSymbol == null)
        //{
        //    // Can't locate the attribute
        //    return idClassesToGenerate;
        //}

        foreach (var declaration in declarationSyntaxes)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var semanticModel = compilation.GetSemanticModel(declaration.SyntaxTree);

            if (semanticModel.GetDeclaredSymbol(declaration) is not INamedTypeSymbol symbol)
            {
                // Have we referenced all the necessary assemblies.
                continue;
            }

            string? wrappedType = null;

            var attributes = symbol.GetAttributes();

            foreach (var attribute in attributes)
            {
                // TODO: Support the generic interface
                //if (attribute.AttributeClass!.Name != MarkerAttribute)
                //{
                //    break;
                //}

                wrappedType = attribute.AttributeClass!.TypeArguments.Single().ToDisplayString();

                break;
            }

            if (wrappedType is null)
            {
                // create a diagnostic!
                continue;
            }
            
            var symbolName = symbol.Name;
            var symbolNamespace = symbol.ContainingNamespace.ToString();

            typesToGenerate.Add(new EntityIdInfo(symbolNamespace, symbolName, wrappedType));
        }

        return typesToGenerate.ToImmutable();
    }
}
