// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionDeclarationProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.DeclarationStage;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.DiscriminatedUnions.Generator;
using Sundew.DiscriminatedUnions.Generator.Unions;
using Sundew.DiscriminatedUnions.Shared;

internal static class DiscriminatedUnionDeclarationProvider
{
    public static IncrementalValuesProvider<DiscriminatedUnionDeclaration> SetupDiscriminatedUnionDeclarationStage(this SyntaxValueProvider syntaxProvider)
    {
        return syntaxProvider.CreateSyntaxProvider(
            static (syntaxNode, _) => IsDiscriminatedUnionCandidate(syntaxNode),
            static (generatorContextSyntax, _) => TryGetDiscriminatedUnionDeclaration(generatorContextSyntax.Node, generatorContextSyntax.SemanticModel)).Where(x => x != null).Select((x, y) => x.GetValueOrDefault());
    }

    internal static DiscriminatedUnionDeclaration? TryGetDiscriminatedUnionDeclaration(SyntaxNode syntaxNode, SemanticModel semanticModel)
    {
        var declaredSymbol = semanticModel.GetDeclaredSymbol(syntaxNode);
        if (syntaxNode is MemberDeclarationSyntax memberDeclarationSyntax && declaredSymbol is ITypeSymbol typeSymbol && TryGetUnionWithFeatures(typeSymbol, out var generatorFeatures, out var isGeneratorDisabled) && typeSymbol.TryGetSupportedAccessibility(out var accessibility))
        {
            IEnumerable<INamedTypeSymbol> baseSymbol = typeSymbol.Interfaces.Concat(typeSymbol.BaseType != null ? new[] { typeSymbol.BaseType } : Array.Empty<INamedTypeSymbol>());
            var isConstrainingUnionOfSameTypeKind = baseSymbol.Where(DiscriminatedUnionExtensions.IsDiscriminatedUnion).Any(x => x.TypeKind == typeSymbol.TypeKind);
            return new DiscriminatedUnionDeclaration(typeSymbol.GetFullType(), typeSymbol.GetUnderlyingType(), accessibility, memberDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword), isConstrainingUnionOfSameTypeKind, isGeneratorDisabled, generatorFeatures);
        }

        return null;
    }

    private static bool TryGetUnionWithFeatures(ITypeSymbol typeSymbol, out GeneratorFeatures generatorFeatures, out bool isGeneratorDisabled)
    {
        var discriminatedUnionAttribute = UnionSymbolHelper.GetDiscriminatedUnionAttribute(typeSymbol);
        generatorFeatures = UnionSymbolHelper.GetGeneratorFeatures(discriminatedUnionAttribute, CancellationToken.None);
        isGeneratorDisabled = UnionSymbolHelper.IsGeneratorDisabled(discriminatedUnionAttribute, CancellationToken.None);
        return discriminatedUnionAttribute?.ApplicationSyntaxReference != null;
    }

    private static bool IsDiscriminatedUnionCandidate(SyntaxNode syntaxNode)
    {
        // A class union has the attribute, so a declaration without attributes is ruled out before anything is asked about its symbol.
        // For a type that is declared in several parts, the part that has the attribute is the one that is found.
        if (syntaxNode is TypeDeclarationSyntax { AttributeLists.Count: > 0 } typeDeclarationSyntax)
        {
            var kind = typeDeclarationSyntax.Kind();
            return kind switch
            {
                SyntaxKind.ClassDeclaration => typeDeclarationSyntax.Modifiers.Any(SyntaxKind.AbstractKeyword),
                SyntaxKind.RecordDeclaration => typeDeclarationSyntax.Modifiers.Any(SyntaxKind.AbstractKeyword),
                SyntaxKind.InterfaceDeclaration => true,
                _ => false,
            };
        }

        return false;
    }
}