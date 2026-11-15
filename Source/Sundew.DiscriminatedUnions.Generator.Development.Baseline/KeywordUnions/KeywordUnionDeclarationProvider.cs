// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeywordUnionDeclarationProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.KeywordUnions;

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.DiscriminatedUnions.Generator.Unions;

/// <summary>
/// Finds the union declarations in a compilation that something is generated for.
/// A union declaration asks for generated members by being partial, and for being segregated with the discriminated union attribute.
/// </summary>
internal static class KeywordUnionDeclarationProvider
{
    /// <summary>
    /// Sets up the stage that provides the union declarations that something is generated for.
    /// </summary>
    /// <param name="syntaxProvider">The syntax provider.</param>
    /// <returns>The union declarations.</returns>
    public static IncrementalValuesProvider<KeywordUnionDeclaration> SetupKeywordUnionDeclarationStage(this SyntaxValueProvider syntaxProvider)
    {
        return syntaxProvider.CreateSyntaxProvider(
                static (syntaxNode, _) => syntaxNode is UnionDeclarationSyntax unionDeclarationSyntax
                                          && (unionDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword) || unionDeclarationSyntax.AttributeLists.Count > 0),
                static (generatorSyntaxContext, cancellationToken) => TryGetKeywordUnionDeclaration(generatorSyntaxContext.Node, generatorSyntaxContext.SemanticModel, cancellationToken))
            .Where(static x => x != null)
            .Select(static (x, _) => x!);
    }

    /// <summary>
    /// Tries to get the union declared by the specified syntax node.
    /// </summary>
    /// <param name="syntaxNode">The syntax node.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The union declaration or <c>null</c>, if the node does not declare one that something should be generated for.</returns>
    internal static KeywordUnionDeclaration? TryGetKeywordUnionDeclaration(SyntaxNode syntaxNode, SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        if (syntaxNode is not UnionDeclarationSyntax unionDeclarationSyntax || semanticModel.GetDeclaredSymbol(unionDeclarationSyntax, cancellationToken) is not { IsUnion: true } typeSymbol)
        {
            return null;
        }

        // A union can be declared in several parts, but only the first of them may produce what is generated.
        var primaryDeclaration = typeSymbol.DeclaringSyntaxReferences.Select(x => x.GetSyntax(cancellationToken)).OfType<UnionDeclarationSyntax>().FirstOrDefault();
        if (primaryDeclaration == null || primaryDeclaration.SyntaxTree != unionDeclarationSyntax.SyntaxTree || primaryDeclaration.Span != unionDeclarationSyntax.Span)
        {
            return null;
        }

        var caseTypes = typeSymbol.UnionCaseTypes;
        var discriminatedUnionAttribute = UnionSymbolHelper.GetDiscriminatedUnionAttribute(typeSymbol);
        if (caseTypes.IsDefaultOrEmpty
            || caseTypes.Any(x => x.TypeKind == TypeKind.Error)
            || UnionSymbolHelper.IsGeneratorDisabled(discriminatedUnionAttribute, cancellationToken))
        {
            return null;
        }

        // Members can only be added to a union that is partial. The types that segregate it are types of their own, which do not need it to be.
        var isPartial = unionDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword);
        var generatedMembers = isPartial ? UnionSymbolHelper.GetGeneratedMembers(typeSymbol) : GeneratedMembers.None;
        var factories = isPartial ? UnionFactoryProvider.GetFactories(typeSymbol, caseTypes, true) : ImmutableArray<UnionFactory>.Empty;
        var segregation = UnionSegregationProvider.GetSegregation(
            typeSymbol,
            caseTypes,
            UnionCaseMatching.Value,
            UnionSymbolHelper.GetGeneratorFeatures(discriminatedUnionAttribute, cancellationToken),
            semanticModel.Compilation,
            unionDeclarationSyntax.Identifier.GetLocation());
        if (generatedMembers == GeneratedMembers.None && factories.IsEmpty && segregation.IsEmpty)
        {
            return null;
        }

        return new KeywordUnionDeclaration(
            UnionType.Create(typeSymbol),
            caseTypes.Select(x => x.WithNullableAnnotation(NullableAnnotation.NotAnnotated).ToDisplayString(CodeAnalysisHelper.FullyQualifiedDisplayFormat)).ToImmutableArray(),
            generatedMembers,
            factories,
            segregation);
    }
}
