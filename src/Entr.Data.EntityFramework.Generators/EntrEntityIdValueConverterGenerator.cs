using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Entr.Data.EntityFramework.Generators;

[Generator]
public class EntrEntityIdValueConverterGenerator : IIncrementalGenerator
{
    private const string MarkerAttribute = "Entr.Domain.EntrEntityIdAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<ClassDeclarationSyntax> classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null)!;

        IncrementalValueProvider<(Compilation Compilation, ImmutableArray<ClassDeclarationSyntax> Classes)> compilationAndClasses
            = context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndClasses,
            static (spc, source) => Execute(source.Compilation, source.Classes, spc));
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node) =>
        node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };

    private static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;

        foreach (var attributeList in classDeclaration.AttributeLists)
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
                    return classDeclaration;
                }
            }
        }

        return null;
    }

    private static void Execute(
        Compilation compilation, 
        ImmutableArray<ClassDeclarationSyntax> locatedClasses, 
        SourceProductionContext context)
    {
        if (locatedClasses.IsDefaultOrEmpty)
        {
            // nothing to do yet
            return;
        }

        var distinctClasses = locatedClasses.Distinct();

        var typesToGenerate = GetTypesToGenerate(compilation, distinctClasses, context.CancellationToken);

        if (typesToGenerate.Any())
        {
            var source = SourceCodeGenerator.GenerateSource(typesToGenerate);

            context.AddSource("EntrEntityFrameworkValueConverters.g.cs", SourceText.From(source, Encoding.UTF8));
        }
    }

    private static List<EntityIdTypeMetadata> GetTypesToGenerate(Compilation compilation, IEnumerable<ClassDeclarationSyntax> classDelarations, CancellationToken cancellationToken)
    {
        var typesToGenerate = new List<EntityIdTypeMetadata>();

        //var markerAttributeSymbol = compilation.GetTypeByMetadataName(MarkerAttribute);

        //if (markerAttributeSymbol == null)
        //{
        //    // Can't locate the attribute
        //    return idClassesToGenerate;
        //}

        foreach (var classDeclaration in classDelarations)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var semanticModel = compilation.GetSemanticModel(classDeclaration.SyntaxTree);

            if (semanticModel.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol classSymbol)
            {
                // Have we referenced all the necessary assemblies.
                continue;
            }

            string? valueType = null;

            var attributes = classSymbol.GetAttributes();

            foreach (var attribute in attributes)
            {
                // TODO: Support the generic interface
                //if (attribute.AttributeClass!.Name != MarkerAttribute)
                //{
                //    break;
                //}

                valueType = attribute.AttributeClass.TypeArguments.Single().ToDisplayString();

                break;
            }

            if (valueType is null)
            {
                // create a diagnostic!
                continue;
            }
            
            var className = classSymbol.Name;
            var classNamespace = classSymbol.ContainingNamespace.ToString();

            typesToGenerate.Add(new EntityIdTypeMetadata(classNamespace, className, valueType));
        }

        return typesToGenerate;
    }
}
