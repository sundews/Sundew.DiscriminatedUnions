// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionDeclarationProvider.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
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
using Sundew.DiscriminatedUnions.Analyzer;
using Sundew.DiscriminatedUnions.Generator;
using Sundew.DiscriminatedUnions.Generator.Extensions;
using Sundew.DiscriminatedUnions.Generator.Model;
using Accessibility = Sundew.DiscriminatedUnions.Generator.Model.Accessibility;
using Type = Sundew.DiscriminatedUnions.Generator.Model.Type;

internal static class DiscriminatedUnionDeclarationProvider
{
    public static IncrementalValuesProvider<DiscriminatedUnionDeclaration> SetupDiscriminatedUnionDeclarationStage(this SyntaxValueProvider syntaxProvider)
    {
        return syntaxProvider.CreateSyntaxProvider(
            static (syntaxNode, _) => IsDiscriminatedUnionCandidate(syntaxNode),
            static (generatorContextSyntax, _) => GetDiscriminatedUnionDeclaration(generatorContextSyntax)).Where(x => x != null).Select((x, y) => x.GetValueOrDefault());
    }

    private static DiscriminatedUnionDeclaration? GetDiscriminatedUnionDeclaration(GeneratorSyntaxContext generatorContextSyntax)
    {
        var declaredSymbol = generatorContextSyntax.SemanticModel.GetDeclaredSymbol(generatorContextSyntax.Node);
        if (generatorContextSyntax.Node is MemberDeclarationSyntax memberDeclarationSyntax && declaredSymbol is ITypeSymbol typeSymbol && TryGetUnionWithFeatures(typeSymbol, out var generatorFeatures) && TryGetSupportedAccessibility(typeSymbol, out var accessibility))
        {
            IEnumerable<INamedTypeSymbol> baseSymbol = typeSymbol.BaseType != null ? new[] { typeSymbol.BaseType } : Array.Empty<INamedTypeSymbol>();
            var isConstrainingUnion = baseSymbol.Concat(typeSymbol.Interfaces).Any(UnionHelper.IsDiscriminatedUnion);
            return new DiscriminatedUnionDeclaration(new Type(typeSymbol.MetadataName, typeSymbol.ContainingNamespace.ToDisplayString(CodeAnalysisHelper.NamespaceQualifiedDisplayFormat)), GetUnderlyingType(typeSymbol), accessibility, memberDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword), isConstrainingUnion, generatorFeatures);
        }

        return null;
    }

    private static bool TryGetUnionWithFeatures(ITypeSymbol typeSymbol, out GeneratorFeatures generatorFeatures)
    {
        var discriminatedUnionAttributeSyntax = typeSymbol.GetAttributes().FirstOrDefault(attribute =>
        {
            var containingClass = attribute.AttributeClass?.ToDisplayString();
            return containingClass == typeof(Sundew.DiscriminatedUnions.DiscriminatedUnion).FullName;
        })?.ApplicationSyntaxReference?.GetSyntax() as AttributeSyntax;

        var generatorFeatureArgumentSyntax = discriminatedUnionAttributeSyntax?.ArgumentList?.Arguments.FirstOrDefault(x =>
            x.NameColon == null || x.NameColon.Name.Identifier.ToString() == "generatorFeatures");
        if (generatorFeatureArgumentSyntax == null)
        {
            generatorFeatures = GeneratorFeatures.None;
            return false;
        }

        generatorFeatures = generatorFeatureArgumentSyntax.Expression.ToFullString().ParseFlagsEnum<GeneratorFeatures>(CultureInfo.InvariantCulture, '|');
        return true;
    }

    private static UnderlyingType GetUnderlyingType(ITypeSymbol typeSymbol)
    {
        return typeSymbol.TypeKind switch
        {
            TypeKind.Class => typeSymbol.IsRecord ? UnderlyingType.Record : UnderlyingType.Class,
            TypeKind.Interface => UnderlyingType.Interface,
            _ => throw new ArgumentOutOfRangeException(nameof(typeSymbol.TypeKind), typeSymbol.TypeKind, FormattableString.Invariant($"Unexpected TypeKind on {typeSymbol.Name}")),
        };
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

    private static bool TryGetSupportedAccessibility(ITypeSymbol typeSymbol, out Accessibility accessibility)
    {
        switch (typeSymbol.DeclaredAccessibility)
        {
            case Microsoft.CodeAnalysis.Accessibility.Internal:
                accessibility = Accessibility.Internal;
                return true;
            case Microsoft.CodeAnalysis.Accessibility.Public:
                accessibility = Accessibility.Public;
                return true;
            default:
                accessibility = default;
                return false;
        }
    }
}