// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionCaseDeclarationProvider.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.DeclarationStage;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.DiscriminatedUnions.Generator;
using Sundew.DiscriminatedUnions.Generator.Model;
using Sundew.DiscriminatedUnions.Generator.OutputStage;
using Sundew.DiscriminatedUnions.Shared;
using Sundew.DiscriminatedUnions.Text;

internal static class DiscriminatedUnionCaseDeclarationProvider
{
    public static IncrementalValuesProvider<DiscriminatedUnionCaseDeclaration> SetupDiscriminatedUnionCaseDeclarationStage(this SyntaxValueProvider syntaxProvider)
    {
        return syntaxProvider.CreateSyntaxProvider(
            static (syntaxNode, _) => IsDiscriminatedUnionCandidate(syntaxNode),
            static (generatorContextSyntax, _) => TryGetDiscriminatedUnionCaseDeclaration(generatorContextSyntax.Node, generatorContextSyntax.SemanticModel)).Where(x => x != null).Select((x, y) => x.GetValueOrDefault());
    }

    internal static DiscriminatedUnionCaseDeclaration? TryGetDiscriminatedUnionCaseDeclaration(SyntaxNode syntaxNode, SemanticModel semanticModel)
    {
        var symbol = semanticModel.GetDeclaredSymbol(syntaxNode);
        if (symbol is INamedTypeSymbol namedTypeSymbol)
        {
            var owners = FindOwners(namedTypeSymbol).Where(x => !SymbolEqualityComparer.Default.Equals(x, namedTypeSymbol.ContainingType)).Select(x => x.GetSourceType()).ToImmutableArray();
            var parameters = TryGetParameters(namedTypeSymbol);
            if (parameters == null)
            {
                return null;
            }

            return new DiscriminatedUnionCaseDeclaration(namedTypeSymbol.GetFullType(), owners, parameters.ToImmutableArray());
        }

        return null;
    }

    private static IEnumerable<Parameter>? TryGetParameters(INamedTypeSymbol namedTypeSymbol)
    {
        var selectedConstructor = namedTypeSymbol.Constructors.SkipWhile(x =>
                x.ContainingType.IsRecord &&
                SymbolEqualityComparer.Default.Equals(x.Parameters.FirstOrDefault()?.Type, x.ContainingType))
            .FirstOrDefault();
        if (selectedConstructor == null)
        {
            return null;
        }

        return selectedConstructor.Parameters.Select(x =>
        {
            var typeName = x.ToDisplayString(CodeAnalysisHelper.FullyQualifiedParameterTypeFormat);
            var defaultValue = x.HasExplicitDefaultValue ? x.ExplicitDefaultValue?.ToString() ?? GeneratorConstants.Null : null;
            return new Parameter(typeName, x.Name.Uncapitalize().AvoidKeywordCollision(), defaultValue);
        });
    }

    private static IEnumerable<INamedTypeSymbol> FindOwners(ITypeSymbol typeSymbol)
    {
        var baseType = typeSymbol.BaseType;
        while (baseType != null)
        {
            if (baseType.IsDiscriminatedUnion())
            {
                yield return baseType;
            }

            baseType = baseType.BaseType;
        }

        foreach (var discriminatedUnionTypeSymbol in typeSymbol.AllInterfaces.Where(DiscriminatedUnionExtensions.IsDiscriminatedUnion))
        {
            yield return discriminatedUnionTypeSymbol;
        }
    }

    private static bool IsDiscriminatedUnionCandidate(SyntaxNode syntaxNode)
    {
        static bool HasBaseListAndIsNotAbstract(TypeDeclarationSyntax typeDeclarationSyntax)
        {
            return typeDeclarationSyntax.BaseList != null && typeDeclarationSyntax.Modifiers.Any(SyntaxKind.SealedKeyword);
        }

        return syntaxNode switch
        {
            ClassDeclarationSyntax classDeclarationSyntax => HasBaseListAndIsNotAbstract(classDeclarationSyntax),
            RecordDeclarationSyntax recordDeclarationSyntax => HasBaseListAndIsNotAbstract(recordDeclarationSyntax) && recordDeclarationSyntax.Kind() == SyntaxKind.RecordDeclaration,
            _ => false,
        };
    }
}