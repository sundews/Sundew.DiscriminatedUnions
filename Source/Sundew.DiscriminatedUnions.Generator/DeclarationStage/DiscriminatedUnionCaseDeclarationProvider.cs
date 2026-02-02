// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionCaseDeclarationProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
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
using Sundew.DiscriminatedUnions.Generator.Model;
using Sundew.DiscriminatedUnions.Generator.OutputStage;
using Sundew.DiscriminatedUnions.Shared;
using Sundew.DiscriminatedUnions.Text;

internal static class DiscriminatedUnionCaseDeclarationProvider
{
    public static IncrementalValuesProvider<DiscriminatedUnionCaseDeclaration> SetupDiscriminatedUnionCaseDeclarationStage(this SyntaxValueProvider syntaxProvider)
    {
        return syntaxProvider.CreateSyntaxProvider(
            static (syntaxNode, _) => IsDiscriminatedUnionCaseCandidate(syntaxNode),
            static (generatorContextSyntax, _) => TryGetDiscriminatedUnionCaseDeclaration(generatorContextSyntax.Node, generatorContextSyntax.SemanticModel)).Where(x => x != null).Select((x, y) => x.GetValueOrDefault());
    }

    internal static DiscriminatedUnionCaseDeclaration? TryGetDiscriminatedUnionCaseDeclaration(SyntaxNode syntaxNode, SemanticModel semanticModel)
    {
        var symbol = semanticModel.GetDeclaredSymbol(syntaxNode);
        if (symbol is INamedTypeSymbol caseNamedTypeSymbol && syntaxNode is MemberDeclarationSyntax memberDeclarationSyntax && caseNamedTypeSymbol.TryGetSupportedAccessibility(out var accessibility))
        {
            var owners = FindOwners(caseNamedTypeSymbol).Select(x => (Type: x.GetSourceType(), ReturnType: x.GetFullType(true), HasConflictingName: HasConflictingName(x, caseNamedTypeSymbol), IsInterface: x.GetUnderlyingType() == UnderlyingType.Interface)).ToImmutableArray();
            var parameters = TryGetParameters(caseNamedTypeSymbol);
            if (parameters == default)
            {
                return default;
            }

            return new DiscriminatedUnionCaseDeclaration(caseNamedTypeSymbol.GetUnderlyingType(), accessibility, caseNamedTypeSymbol.GetFullType(), owners, parameters.ToImmutableArray(), memberDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword));
        }

        return default;
    }

    private static bool HasConflictingName(INamedTypeSymbol discriminatedUnionNameTypeSymbol, INamedTypeSymbol caseNamedTypeSymbol)
    {
        var members = discriminatedUnionNameTypeSymbol.GetMembers();
        return members.Any(x => SymbolEqualityComparer.Default.Equals(x, caseNamedTypeSymbol));
    }

    private static IEnumerable<Parameter>? TryGetParameters(INamedTypeSymbol namedTypeSymbol)
    {
        var selectedConstructor = namedTypeSymbol.Constructors
            .OrderByDescending(x => x.Parameters.Length)
            .SkipWhile(x =>
                x.ContainingType.IsRecord &&
                SymbolEqualityComparer.Default.Equals(x.Parameters.FirstOrDefault()?.Type, x.ContainingType))
            .FirstOrDefault();
        if (selectedConstructor == default)
        {
            return default;
        }

        return selectedConstructor.Parameters.Select(x =>
        {
            string GetDefaultValue(IParameterSymbol parameterSymbol)
            {
                return (parameterSymbol.ExplicitDefaultValue, parameterSymbol.Type.TypeKind) switch
                {
                    (bool value, _) => value ? GeneratorConstants.True : GeneratorConstants.False,
                    ({ } enumValue, TypeKind.Enum) => x.Type.GetMembers().OfType<IFieldSymbol>().Where(x => x.IsStatic).First(x => x.ConstantValue == enumValue).ToDisplayString(CodeAnalysisHelper.FullyQualifiedParameterTypeFormat),
                    ({ } enumValue, _) => enumValue.ToString(),
                    _ => GeneratorConstants.Default,
                };
            }

            var typeName = x.Type.ToDisplayString(CodeAnalysisHelper.FullyQualifiedParameterTypeFormat);
            var defaultValue = x.HasExplicitDefaultValue ? GetDefaultValue(x) : default;
            return new Parameter(typeName, x.Name.Uncapitalize().AvoidKeywordCollision(), defaultValue);
        });
    }

    private static IEnumerable<INamedTypeSymbol> FindOwners(ITypeSymbol typeSymbol)
    {
        var baseType = typeSymbol.BaseType;
        while (baseType != default)
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

    private static bool IsDiscriminatedUnionCaseCandidate(SyntaxNode syntaxNode)
    {
        static bool HasBaseListAndIsNotAbstract(TypeDeclarationSyntax typeDeclarationSyntax)
        {
            return typeDeclarationSyntax.BaseList != default && typeDeclarationSyntax.Modifiers.Any(SyntaxKind.SealedKeyword);
        }

        return syntaxNode switch
        {
            ClassDeclarationSyntax classDeclarationSyntax => HasBaseListAndIsNotAbstract(classDeclarationSyntax),
            RecordDeclarationSyntax recordDeclarationSyntax => HasBaseListAndIsNotAbstract(recordDeclarationSyntax) && recordDeclarationSyntax.Kind() == SyntaxKind.RecordDeclaration,
            _ => false,
        };
    }
}