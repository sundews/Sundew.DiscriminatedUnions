// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClosedTypeCaseDeclarationProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.ClosedTypes;

using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.DiscriminatedUnions.Generator.Unions;
using Sundew.DiscriminatedUnions.Shared;

/// <summary>
/// Finds the cases of partial closed types that are partial themselves, which is how a case asks for a list of cases of its own.
/// </summary>
internal static class ClosedTypeCaseDeclarationProvider
{
    private const string CasesName = "Cases";

    /// <summary>
    /// Sets up the stage that provides the partial cases of partial closed types.
    /// </summary>
    /// <param name="syntaxProvider">The syntax provider.</param>
    /// <returns>The cases.</returns>
    public static IncrementalValuesProvider<ClosedTypeCaseDeclaration> SetupClosedTypeCaseDeclarationStage(this SyntaxValueProvider syntaxProvider)
    {
        return syntaxProvider.CreateSyntaxProvider(
                static (syntaxNode, _) => IsCaseCandidate(syntaxNode),
                static (generatorSyntaxContext, cancellationToken) => TryGetClosedTypeCaseDeclaration(generatorSyntaxContext.Node, generatorSyntaxContext.SemanticModel, cancellationToken))
            .Where(static x => x != null)
            .Select(static (x, _) => x!);
    }

    /// <summary>
    /// Tries to get the case of a closed type declared by the specified syntax node.
    /// </summary>
    /// <param name="syntaxNode">The syntax node.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The case or <c>null</c>, if the node does not declare one that a list of cases should be generated for.</returns>
    internal static ClosedTypeCaseDeclaration? TryGetClosedTypeCaseDeclaration(SyntaxNode syntaxNode, SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        if (syntaxNode is not TypeDeclarationSyntax typeDeclarationSyntax
            || semanticModel.GetDeclaredSymbol(typeDeclarationSyntax, cancellationToken) is not { TypeKind: TypeKind.Class, IsClosed: false, BaseType.IsClosed: true } typeSymbol)
        {
            return null;
        }

        // A case can be declared in several parts, but only the first of those that name what it derives from may produce the generated part.
        var primaryDeclaration = typeSymbol.DeclaringSyntaxReferences.Select(x => x.GetSyntax(cancellationToken)).FirstOrDefault(IsCaseCandidate);
        if (primaryDeclaration == null
            || primaryDeclaration.SyntaxTree != typeDeclarationSyntax.SyntaxTree
            || primaryDeclaration.Span != typeDeclarationSyntax.Span
            || typeSymbol.GetMembers(CasesName).Any(x => !x.IsImplicitlyDeclared)
            || !IsNestedInPartialTypes(typeSymbol, cancellationToken)
            || !IsCaseOfPartialClosedType(typeSymbol, cancellationToken)
            || IsCaseOfClassUnion(typeSymbol))
        {
            return null;
        }

        return new ClosedTypeCaseDeclaration(UnionType.Create(typeSymbol), typeSymbol.IsRecord, typeSymbol.ToDisplayString(CodeAnalysisHelper.FullyQualifiedDisplayFormat));
    }

    private static bool IsCaseCandidate(SyntaxNode syntaxNode)
    {
        return syntaxNode is TypeDeclarationSyntax { BaseList: not null } typeDeclarationSyntax
               && (typeDeclarationSyntax.IsKind(SyntaxKind.ClassDeclaration) || typeDeclarationSyntax.IsKind(SyntaxKind.RecordDeclaration))
               && typeDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword)
               && !typeDeclarationSyntax.Modifiers.Any(SyntaxKind.ClosedKeyword);
    }

    private static bool IsCaseOfPartialClosedType(INamedTypeSymbol caseSymbol, CancellationToken cancellationToken)
    {
        // A case is a case of the closed type it derives from and of the closed types that one derives from. One of them has to ask for generated members.
        for (var baseType = caseSymbol.BaseType; baseType is { IsClosed: true }; baseType = baseType.BaseType)
        {
            var closedType = baseType.OriginalDefinition;
            if (IsPartial(closedType, cancellationToken) && !UnionSymbolHelper.IsGeneratorDisabled(UnionSymbolHelper.GetDiscriminatedUnionAttribute(closedType), cancellationToken))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsNestedInPartialTypes(INamedTypeSymbol typeSymbol, CancellationToken cancellationToken)
    {
        // The generated part of the case is declared in a part of each type the case is nested in.
        for (var containingType = typeSymbol.ContainingType; containingType != null; containingType = containingType.ContainingType)
        {
            if (!IsPartial(containingType, cancellationToken))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsPartial(INamedTypeSymbol typeSymbol, CancellationToken cancellationToken)
    {
        return typeSymbol.DeclaringSyntaxReferences.Select(x => x.GetSyntax(cancellationToken)).OfType<MemberDeclarationSyntax>().Any(x => x.Modifiers.Any(SyntaxKind.PartialKeyword));
    }

    private static bool IsCaseOfClassUnion(INamedTypeSymbol typeSymbol)
    {
        // A case of a class union gets its list from the generator for class unions.
        for (var baseType = typeSymbol.BaseType; baseType != null; baseType = baseType.BaseType)
        {
            if (baseType.IsDiscriminatedUnion())
            {
                return true;
            }
        }

        return typeSymbol.AllInterfaces.Any(x => x.IsDiscriminatedUnion());
    }
}
