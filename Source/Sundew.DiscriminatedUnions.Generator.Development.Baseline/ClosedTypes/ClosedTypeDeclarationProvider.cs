// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClosedTypeDeclarationProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.ClosedTypes;

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.DiscriminatedUnions.Generator.Unions;

/// <summary>
/// Finds the closed classes and records in a compilation that something is generated for.
/// A closed type asks for generated members by being partial, and for being segregated with the discriminated union attribute.
/// </summary>
internal static class ClosedTypeDeclarationProvider
{
    /// <summary>
    /// Sets up the stage that provides the closed types that something is generated for.
    /// </summary>
    /// <param name="syntaxProvider">The syntax provider.</param>
    /// <returns>The closed types.</returns>
    public static IncrementalValuesProvider<ClosedTypeDeclaration> SetupClosedTypeDeclarationStage(this SyntaxValueProvider syntaxProvider)
    {
        return syntaxProvider.CreateSyntaxProvider(
                static (syntaxNode, _) => IsClosedTypeCandidate(syntaxNode),
                static (generatorSyntaxContext, cancellationToken) => TryGetClosedTypeDeclaration(generatorSyntaxContext.Node, generatorSyntaxContext.SemanticModel, cancellationToken))
            .Where(static x => x != null)
            .Select(static (x, _) => x!);
    }

    /// <summary>
    /// Tries to get the closed type declared by the specified syntax node.
    /// </summary>
    /// <param name="syntaxNode">The syntax node.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The closed type or <c>null</c>, if the node does not declare one that something should be generated for.</returns>
    internal static ClosedTypeDeclaration? TryGetClosedTypeDeclaration(SyntaxNode syntaxNode, SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        if (syntaxNode is not TypeDeclarationSyntax typeDeclarationSyntax || semanticModel.GetDeclaredSymbol(typeDeclarationSyntax, cancellationToken) is not { IsClosed: true } typeSymbol)
        {
            return null;
        }

        // A closed type can be declared in several parts, but only the first of those that say it is closed may produce what is generated.
        var primaryDeclaration = typeSymbol.DeclaringSyntaxReferences.Select(x => x.GetSyntax(cancellationToken)).FirstOrDefault(IsClosedTypeCandidate);
        var discriminatedUnionAttribute = UnionSymbolHelper.GetDiscriminatedUnionAttribute(typeSymbol);
        if (primaryDeclaration == null
            || primaryDeclaration.SyntaxTree != typeDeclarationSyntax.SyntaxTree
            || primaryDeclaration.Span != typeDeclarationSyntax.Span
            || UnionSymbolHelper.IsGeneratorDisabled(discriminatedUnionAttribute, cancellationToken))
        {
            return null;
        }

        var caseTypes = UnionSymbolHelper.GetClosedTypeCases(typeSymbol, cancellationToken);
        if (caseTypes.IsEmpty)
        {
            return null;
        }

        // Members can only be added to a closed type that is partial. The types that segregate it are types of their own, which do not need it to be.
        var isPartial = typeDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword);
        var generatedMembers = isPartial ? UnionSymbolHelper.GetGeneratedMembers(typeSymbol) & GeneratedMembers.Cases : GeneratedMembers.None;
        var factories = isPartial ? UnionFactoryProvider.GetClosedTypeFactories(typeSymbol, caseTypes) : ImmutableArray<UnionFactory>.Empty;
        var segregation = UnionSegregationProvider.GetSegregation(
            typeSymbol,
            caseTypes,
            UnionCaseMatching.Type,
            UnionSymbolHelper.GetGeneratorFeatures(discriminatedUnionAttribute, cancellationToken),
            semanticModel.Compilation,
            typeDeclarationSyntax.Identifier.GetLocation());
        if (generatedMembers == GeneratedMembers.None && factories.IsEmpty && segregation.IsEmpty)
        {
            return null;
        }

        return new ClosedTypeDeclaration(
            UnionType.Create(typeSymbol),
            typeSymbol.IsRecord,
            typeSymbol.BaseType is { SpecialType: not SpecialType.System_Object },
            caseTypes.Select(x => x.WithNullableAnnotation(NullableAnnotation.NotAnnotated).ToDisplayString(CodeAnalysisHelper.FullyQualifiedDisplayFormat)).ToImmutableArray(),
            generatedMembers,
            factories,
            segregation);
    }

    private static bool IsClosedTypeCandidate(SyntaxNode syntaxNode)
    {
        return syntaxNode is TypeDeclarationSyntax typeDeclarationSyntax
               && typeDeclarationSyntax.Modifiers.Any(SyntaxKind.ClosedKeyword)
               && (typeDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword) || typeDeclarationSyntax.AttributeLists.Count > 0);
    }
}
