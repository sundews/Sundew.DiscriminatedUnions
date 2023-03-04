// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionCaseDeclarationProvider.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.DeclarationStage;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.DiscriminatedUnions.Analyzer;
using Sundew.DiscriminatedUnions.Generator;
using Sundew.DiscriminatedUnions.Generator.Model;
using Sundew.DiscriminatedUnions.Text;
using Type = Sundew.DiscriminatedUnions.Generator.Model.Type;

internal static class DiscriminatedUnionCaseDeclarationProvider
{
    public static IncrementalValuesProvider<DiscriminatedUnionCaseDeclaration> SetupDiscriminatedUnionCaseDeclarationStage(this SyntaxValueProvider syntaxProvider)
    {
        return syntaxProvider.CreateSyntaxProvider(
            static (syntaxNode, _) => IsDiscriminatedUnionCandidate(syntaxNode),
            static (generatorContextSyntax, _) => TryGetDiscriminatedUnionCaseDeclaration(generatorContextSyntax)).Where(x => x != null).Select((x, y) => x.GetValueOrDefault());
    }

    private static DiscriminatedUnionCaseDeclaration? TryGetDiscriminatedUnionCaseDeclaration(GeneratorSyntaxContext generatorContextSyntax)
    {
        var symbol = generatorContextSyntax.SemanticModel.GetDeclaredSymbol(generatorContextSyntax.Node);
        if (symbol is INamedTypeSymbol namedTypeSymbol)
        {
            var owners = FindOwners(namedTypeSymbol).Select(x => x.GetSourceType());
            var parameters = TryGetParameters(namedTypeSymbol);
            if (parameters == null)
            {
                return null;
            }

            return new DiscriminatedUnionCaseDeclaration(namedTypeSymbol.GetSourceType(), owners.ToImmutableArray(), parameters.ToImmutableArray());
        }

        return null;
    }

    private static IEnumerable<Parameter>? TryGetParameters(INamedTypeSymbol namedTypeSymbol)
    {
        var selectedConstructor = namedTypeSymbol.Constructors.OrderByDescending(x => x.Parameters.Length).FirstOrDefault();
        if (selectedConstructor == null)
        {
            return null;
        }

        return selectedConstructor.Parameters.Select(x =>
            new Parameter(x.Type.GetSourceType(), x.Name.Uncapitalize().AvoidKeywordCollision()));
    }

    private static IEnumerable<INamedTypeSymbol> FindOwners(ITypeSymbol typeSymbol)
    {
        var baseType = typeSymbol.BaseType;
        while (baseType != null)
        {
            if (UnionHelper.IsDiscriminatedUnion(baseType))
            {
                yield return baseType;
            }

            baseType = baseType.BaseType;
        }

        foreach (var discriminatedUnionTypeSymbol in typeSymbol.AllInterfaces.Where(UnionHelper.IsDiscriminatedUnion))
        {
            yield return discriminatedUnionTypeSymbol;
        }
    }

    private static bool IsDiscriminatedUnionCandidate(SyntaxNode syntaxNode)
    {
        static bool HasBaseListAndIsNotAbstract(TypeDeclarationSyntax typeDeclarationSyntax)
        {
            return typeDeclarationSyntax.BaseList != null && !typeDeclarationSyntax.Modifiers.Any(SyntaxKind.AbstractKeyword);
        }

        return syntaxNode switch
        {
            ClassDeclarationSyntax classDeclarationSyntax => HasBaseListAndIsNotAbstract(classDeclarationSyntax),
            RecordDeclarationSyntax recordDeclarationSyntax => HasBaseListAndIsNotAbstract(recordDeclarationSyntax) && recordDeclarationSyntax.Kind() == SyntaxKind.RecordDeclaration,
            _ => false,
        };
    }
}