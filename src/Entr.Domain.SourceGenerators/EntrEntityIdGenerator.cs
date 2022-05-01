using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Entr.Domain.SourceGenerators;

[Generator]
public class EntrEntityIdGenerator : IIncrementalGenerator
{
    private const string MarkerAttribute = "Entr.Domain.EntrEntityIdAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "EntityIdExtensionsAttribute.g.cs", SourceText.From(SourceCodeGenerator.Attribute, Encoding.UTF8)));

        // Do a simple filter for enums
        IncrementalValuesProvider<ClassDeclarationSyntax> classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s), // select enums with attributes
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx)) // sect the enum with the [EntrEntityId] attribute
            .Where(static m => m is not null)!; // filter out attributed enums that we don't care about

        // Combine the selected enums with the `Compilation`
        IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> compilationAndClasses
            = context.CompilationProvider.Combine(classDeclarations.Collect());

        // Generate the source using the compilation and enums
        context.RegisterSourceOutput(compilationAndClasses,
            static (spc, source) => Execute(source.Item1, source.Item2, spc));
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node) =>
        node is ClassDeclarationSyntax m && m.AttributeLists.Count > 0;

    private static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        // we know the node is a ClassDeclarationSyntax thanks to IsSyntaxTargetForGeneration
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

        // loop through all the attributes on the method
        foreach (var attributeListSyntax in classDeclarationSyntax.AttributeLists)
        {
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                {
                    // weird, we couldn't get the symbol, ignore it
                    continue;
                }

                var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                var fullName = attributeContainingTypeSymbol.ToDisplayString();

                // Is the attribute the [EnumExtensions] attribute?
                if (fullName.StartsWith(MarkerAttribute + "<"))
                {
                    // return the enum
                    return classDeclarationSyntax;
                }
            }
        }

        // we didn't find the attribute we were looking for
        return null;
    }

    private static void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> idClasses, SourceProductionContext context)
    {
        if (idClasses.IsDefaultOrEmpty)
        {
            // nothing to do yet
            return;
        }

        // I'm not sure if this is actually necessary, but `[LoggerMessage]` does it, so seems like a good idea!
        var distinctClasses = idClasses.Distinct();

        // Convert each ClassDeclarationSyntax to an EnumToGenerate
        var classesToGenerate = GetTypesToGenerate(compilation, distinctClasses, context.CancellationToken);

        // If there were errors in the EnumDeclarationSyntax, we won't create an
        // EnumToGenerate for it, so make sure we have something to generate
        if (classesToGenerate.Any())
        {
            // generate the source code and add it to the output
            var result = SourceCodeGenerator.GenerateEntityIdClass(classesToGenerate);

            context.AddSource("EntrEntityIds.g.cs", SourceText.From(result, Encoding.UTF8));
        }
    }

    private static List<EntityIdToGenerate> GetTypesToGenerate(Compilation compilation, IEnumerable<ClassDeclarationSyntax> classes, CancellationToken cancellationToken)
    {
        // Create a list to hold our output
        var idClassesToGenerate = new List<EntityIdToGenerate>();
        // Get the semantic representation of our marker attribute 
        //var markerAttributeSymbol = compilation.GetTypeByMetadataName(MarkerAttribute);

        //if (markerAttributeSymbol == null)
        //{
        //    // If this is null, the compilation couldn't find the marker attribute type
        //    // which suggests there's something very wrong! Bail out..
        //    return idClassesToGenerate;
        //}

        foreach (var classDeclarationSyntax in classes)
        {
            // stop if we're asked to
            cancellationToken.ThrowIfCancellationRequested();

            // Get the semantic representation of the enum syntax
            var semanticModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);

            if (semanticModel.GetDeclaredSymbol(classDeclarationSyntax) is not INamedTypeSymbol classSymbol)
            {
                // something went wrong, bail out
                continue;
            }

            string? valueType = null;

            var attributes = classSymbol.GetAttributes();

            foreach (var attribute in attributes)
            {
                //if (attribute.AttributeClass!.Name != MarkerAttribute)
                //{
                //    break;
                //}

                valueType = attribute.AttributeClass.TypeArguments.Single().Name;

                break;
            }

            // Get the full type name of the enum e.g. Colour, 
            // or OuterClass<T>.Colour if it was nested in a generic type (for example)
            var className = classSymbol.Name;
            var classNamespace = classSymbol.ContainingNamespace.ToString();

            // Create an EnumToGenerate for use in the generation phase
            idClassesToGenerate.Add(new EntityIdToGenerate(classNamespace, className, valueType));
        }

        return idClassesToGenerate;
    }
}
