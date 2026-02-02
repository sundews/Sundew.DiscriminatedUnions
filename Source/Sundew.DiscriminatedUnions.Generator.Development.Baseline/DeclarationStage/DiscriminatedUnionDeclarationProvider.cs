// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionDeclarationProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.DeclarationStage;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.Base;
using Sundew.DiscriminatedUnions.Generator;
using Sundew.DiscriminatedUnions.Shared;

internal static class DiscriminatedUnionDeclarationProvider
{
    private const string GeneratorFeaturesText = "generatorFeatures";

    public static IncrementalValuesProvider<DiscriminatedUnionDeclaration> SetupDiscriminatedUnionDeclarationStage(this SyntaxValueProvider syntaxProvider)
    {
        return syntaxProvider.CreateSyntaxProvider(
            static (syntaxNode, _) => IsDiscriminatedUnionCandidate(syntaxNode),
            static (generatorContextSyntax, _) => TryGetDiscriminatedUnionDeclaration(generatorContextSyntax.Node, generatorContextSyntax.SemanticModel)).Where(x => x != null).Select((x, y) => x.GetValueOrDefault());
    }

    internal static DiscriminatedUnionDeclaration? TryGetDiscriminatedUnionDeclaration(SyntaxNode syntaxNode, SemanticModel semanticModel)
    {
        var declaredSymbol = semanticModel.GetDeclaredSymbol(syntaxNode);
        if (syntaxNode is MemberDeclarationSyntax memberDeclarationSyntax && declaredSymbol is ITypeSymbol typeSymbol && TryGetUnionWithFeatures(typeSymbol, out var generatorFeatures) && typeSymbol.TryGetSupportedAccessibility(out var accessibility))
        {
            IEnumerable<INamedTypeSymbol> baseSymbol = typeSymbol.BaseType != null ? new[] { typeSymbol.BaseType } : Array.Empty<INamedTypeSymbol>();
            var isConstrainingUnion = baseSymbol.Any(DiscriminatedUnionExtensions.IsDiscriminatedUnion);
            return new DiscriminatedUnionDeclaration(typeSymbol.GetFullType(), typeSymbol.GetUnderlyingType(), accessibility, memberDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword), isConstrainingUnion, generatorFeatures);
        }

        return null;
    }

    private static bool TryGetUnionWithFeatures(ITypeSymbol typeSymbol, out GeneratorFeatures generatorFeatures)
    {
        generatorFeatures = GeneratorFeatures.None;
        var discriminatedUnionAttributeSyntax = typeSymbol.GetAttributes().FirstOrDefault(attribute =>
        {
            var containingClass = attribute.AttributeClass?.ToDisplayString();
            return containingClass == typeof(Sundew.DiscriminatedUnions.DiscriminatedUnion).FullName;
        })?.ApplicationSyntaxReference?.GetSyntax() as AttributeSyntax;

        if (discriminatedUnionAttributeSyntax == null)
        {
            return false;
        }

        var generatorFeatureArgumentSyntax = discriminatedUnionAttributeSyntax?.ArgumentList?.Arguments.FirstOrDefault(x =>
            x.NameColon == null || x.NameColon.Name.Identifier.ToString() == GeneratorFeaturesText);
        if (generatorFeatureArgumentSyntax == null)
        {
            return true;
        }

        generatorFeatures = generatorFeatureArgumentSyntax.Expression.ToFullString().ParseFlagsEnum<GeneratorFeatures>(CultureInfo.InvariantCulture, '|');
        return true;
    }

    private static bool IsDiscriminatedUnionCandidate(SyntaxNode syntaxNode)
    {
        if (syntaxNode is TypeDeclarationSyntax typeDeclarationSyntax)
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