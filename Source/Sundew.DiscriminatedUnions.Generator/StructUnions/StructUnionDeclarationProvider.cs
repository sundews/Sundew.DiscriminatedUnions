// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructUnionDeclarationProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.StructUnions;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.Base.Collections.Immutable;
using Sundew.DiscriminatedUnions.Generator.Unions;
using Sundew.DiscriminatedUnions.Shared;

/// <summary>
/// Finds the struct unions in a compilation: partial structs that list their cases as <see cref="ICase{TCase}"/> interfaces.
/// </summary>
internal static class StructUnionDeclarationProvider
{
    private const string ListSeparator = ", ";
    private const string ArraySuffix = "Array";
    private const string FallbackCaseName = "Case";

    /// <summary>
    /// Sets up the stage that provides the struct union declarations.
    /// </summary>
    /// <param name="syntaxProvider">The syntax provider.</param>
    /// <returns>The struct union declarations.</returns>
    public static IncrementalValuesProvider<StructUnionDeclaration> SetupStructUnionDeclarationStage(this SyntaxValueProvider syntaxProvider)
    {
        return syntaxProvider.CreateSyntaxProvider(
                static (syntaxNode, _) => IsStructUnionCandidate(syntaxNode),
                static (generatorSyntaxContext, cancellationToken) => TryGetStructUnionDeclaration(generatorSyntaxContext.Node, generatorSyntaxContext.SemanticModel, cancellationToken))
            .Where(static x => x != null)
            .Select(static (x, _) => x!);
    }

    /// <summary>
    /// Tries to get the struct union declared by the specified syntax node.
    /// </summary>
    /// <param name="syntaxNode">The syntax node.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The struct union declaration or <c>null</c>, if the node does not declare one.</returns>
    internal static StructUnionDeclaration? TryGetStructUnionDeclaration(SyntaxNode syntaxNode, SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        if (syntaxNode is not TypeDeclarationSyntax typeDeclarationSyntax || semanticModel.GetDeclaredSymbol(typeDeclarationSyntax, cancellationToken) is not { } typeSymbol)
        {
            return null;
        }

        var cases = GetCases(typeSymbol);
        if (cases.Count == 0 || cases.Any(x => x.Type.TypeKind == TypeKind.Error))
        {
            return null;
        }

        // A struct can be declared in several parts, but only one of them may produce the union.
        var declarations = typeSymbol.DeclaringSyntaxReferences.Select(x => x.GetSyntax(cancellationToken)).OfType<TypeDeclarationSyntax>().ToArray();
        var primaryDeclaration = declarations.FirstOrDefault(x => x.BaseList != null);
        if (primaryDeclaration == null || primaryDeclaration.SyntaxTree != typeDeclarationSyntax.SyntaxTree || primaryDeclaration.Span != typeDeclarationSyntax.Span)
        {
            return null;
        }

        var discriminatedUnionAttribute = UnionSymbolHelper.GetDiscriminatedUnionAttribute(typeSymbol);
        if (UnionSymbolHelper.IsGeneratorDisabled(discriminatedUnionAttribute, cancellationToken))
        {
            return null;
        }

        var compilation = semanticModel.Compilation;
        var structUnionCaseClassifier = new StructUnionCaseClassifier(compilation);
        var structUnionCases = cases.Select((_, index) => CreateCase(cases, index, typeSymbol.IsGenericType, structUnionCaseClassifier, compilation)).ToImmutableArray();
        var caseTypes = cases.Select(x => x.Type).ToArray();
        var segregation = UnionSegregationProvider.GetSegregation(
            typeSymbol,
            cases.Select(x => x.DeclaredType).ToArray(),
            UnionCaseMatching.TryGetValue,
            UnionSymbolHelper.GetGeneratorFeatures(discriminatedUnionAttribute, cancellationToken),
            compilation,
            typeDeclarationSyntax.Identifier.GetLocation());
        var diagnostics = GetDiagnostics(typeSymbol, declarations, cases, structUnionCases);

        // The location is only kept when there is something to report at it. A union is compared to what it was before to decide whether its source is generated again,
        // and with its location it would differ every time a line is added above it.
        return new StructUnionDeclaration(
            UnionType.Create(typeSymbol),
            typeSymbol.IsReadOnly,
            !typeSymbol.IsGenericType,
            structUnionCases,
            UnionSymbolHelper.GetGeneratedMembers(typeSymbol),
            UnionFactoryProvider.GetFactories(typeSymbol, caseTypes, false),
            segregation,
            diagnostics,
            diagnostics.Count > 0 ? DiagnosticLocation.From(typeDeclarationSyntax.Identifier.GetLocation()) : null);
    }

    private static bool IsStructUnionCandidate(SyntaxNode syntaxNode)
    {
        return syntaxNode is TypeDeclarationSyntax { BaseList: not null } typeDeclarationSyntax
               && (typeDeclarationSyntax.IsKind(SyntaxKind.StructDeclaration) || typeDeclarationSyntax.IsKind(SyntaxKind.RecordStructDeclaration));
    }

    private static List<CaseType> GetCases(INamedTypeSymbol typeSymbol)
    {
        var cases = new List<CaseType>();
        foreach (var interfaceSymbol in typeSymbol.Interfaces)
        {
            var isDefault = interfaceSymbol.IsDefaultCaseInterface();
            if (!isDefault && !interfaceSymbol.IsCaseInterface())
            {
                continue;
            }

            // A nullable value type is stored as the type it wraps, because the union has no value when the nullable has none.
            var declaredType = interfaceSymbol.TypeArguments[0].WithNullableAnnotation(NullableAnnotation.NotAnnotated);
            var isNullable = declaredType is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T };
            var caseType = isNullable ? ((INamedTypeSymbol)declaredType).TypeArguments[0] : declaredType;
            var index = cases.FindIndex(x => SymbolEqualityComparer.Default.Equals(x.Type, caseType));
            if (index < 0)
            {
                cases.Add(new CaseType(caseType, declaredType, isDefault));
            }
            else
            {
                // A type that is declared both as it is and as nullable is a single case, which can be created from either.
                var declaredCase = cases[index];
                cases[index] = new CaseType(caseType, isNullable ? declaredType : declaredCase.DeclaredType, isDefault || declaredCase.IsDefault);
            }
        }

        return cases;
    }

    private static StructUnionCase CreateCase(IReadOnlyList<CaseType> cases, int index, bool isUnionGeneric, StructUnionCaseClassifier structUnionCaseClassifier, Compilation compilation)
    {
        var (type, declaredType, isDefault) = cases[index];
        var kind = structUnionCaseClassifier.GetKind(type);
        var (assignableCases, castableCases) = GetMatchingCases(cases, index, compilation);
        return new StructUnionCase(
            type.ToDisplayString(CodeAnalysisHelper.FullyQualifiedDisplayFormat),
            declaredType.ToDisplayString(UnionSymbolHelper.DisplayNameFormat),
            GetSimpleName(type),
            kind,
            isDefault,
            kind == StructUnionCaseKind.Unmanaged && (!isUnionGeneric || CanBeStoredOutsideTheUnion(type)),
            !SymbolEqualityComparer.Default.Equals(type, declaredType),
            assignableCases,
            castableCases);
    }

    private static (ImmutableArray<int> AssignableCases, ImmutableArray<int> CastableCases) GetMatchingCases(IReadOnlyList<CaseType> cases, int index, Compilation compilation)
    {
        // The compiler matches a union by the type of the value it contains and not by the case it was created from.
        // So a case also matches the values of the other cases that are of its type: always when their type converts to it, and after a type check when it converts to theirs.
        var assignableCases = ImmutableArray.CreateBuilder<int>();
        var castableCases = ImmutableArray.CreateBuilder<int>();
        for (var otherIndex = 0; otherIndex < cases.Count; otherIndex++)
        {
            if (otherIndex == index)
            {
                continue;
            }

            var conversion = compilation.ClassifyConversion(cases[otherIndex].Type, cases[index].Type);
            if (conversion.IsBoxing || (conversion.IsReference && conversion.IsImplicit))
            {
                assignableCases.Add(otherIndex);
            }
            else if (conversion.IsUnboxing || (conversion.IsReference && conversion.IsExplicit))
            {
                castableCases.Add(otherIndex);
            }
        }

        return (assignableCases.ToImmutable(), castableCases.ToImmutable());
    }

    private static bool CanBeStoredOutsideTheUnion(ITypeSymbol type)
    {
        // A generic type cannot have explicit layout, so the storage that the cases of a generic union share is a struct that is declared next to the union.
        // That struct has no type parameters and is not nested in the union, so it only holds the cases that need neither.
        return !MentionsTypeParameter(type) && UnionSymbolHelper.GetNamespaceAccessibility(type) != Accessibility.NotApplicable;
    }

    private static bool MentionsTypeParameter(ITypeSymbol type)
    {
        switch (type)
        {
            case IArrayTypeSymbol arrayTypeSymbol:
                return MentionsTypeParameter(arrayTypeSymbol.ElementType);
            case INamedTypeSymbol namedTypeSymbol:
                for (var namedType = namedTypeSymbol; namedType != null; namedType = namedType.ContainingType)
                {
                    if (namedType.TypeArguments.Any(MentionsTypeParameter))
                    {
                        return true;
                    }
                }

                return false;
            default:
                // A type parameter mentions itself. For what remains, such as a function pointer, nothing is assumed.
                return true;
        }
    }

    private static string GetSimpleName(ITypeSymbol type)
    {
        if (type is IArrayTypeSymbol arrayTypeSymbol)
        {
            return GetSimpleName(arrayTypeSymbol.ElementType) + ArraySuffix;
        }

        return string.IsNullOrEmpty(type.Name) ? FallbackCaseName : type.Name;
    }

    private static ValueArray<StructUnionDiagnostic> GetDiagnostics(
        INamedTypeSymbol typeSymbol,
        IReadOnlyList<TypeDeclarationSyntax> declarations,
        IReadOnlyList<CaseType> cases,
        ImmutableArray<StructUnionCase> structUnionCases)
    {
        var diagnostics = ImmutableArray.CreateBuilder<StructUnionDiagnostic>();
        if (typeSymbol.IsRecord || typeSymbol.IsRefLikeType || declarations.Any(x => !x.Modifiers.Any(SyntaxKind.PartialKeyword)))
        {
            diagnostics.Add(new StructUnionDiagnostic(StructUnionProblem.NotPartialStruct, string.Empty));
        }

        var typeParameterCases = structUnionCases.Where((_, index) => cases[index].Type.TypeKind == TypeKind.TypeParameter).ToArray();
        if (typeParameterCases.Length > 0)
        {
            diagnostics.Add(new StructUnionDiagnostic(StructUnionProblem.TypeParameterCase, JoinNames(typeParameterCases)));
        }

        var defaultCases = structUnionCases.Where(x => x.IsDefault).ToArray();
        if (defaultCases.Length > 1)
        {
            diagnostics.Add(new StructUnionDiagnostic(StructUnionProblem.MultipleDefaultCases, JoinNames(defaultCases)));
        }

        var defaultCasesWithPayload = defaultCases.Where(x => x.Kind != StructUnionCaseKind.PayloadFree).ToArray();
        if (defaultCasesWithPayload.Length > 0)
        {
            diagnostics.Add(new StructUnionDiagnostic(StructUnionProblem.DefaultCaseHasPayload, JoinNames(defaultCasesWithPayload)));
        }

        if (structUnionCases.All(x => x.Kind == StructUnionCaseKind.Reference))
        {
            diagnostics.Add(new StructUnionDiagnostic(StructUnionProblem.AllCasesAreReferenceTypes, JoinNames(structUnionCases)));
        }

        return diagnostics.ToImmutable();
    }

    private static string JoinNames(IEnumerable<StructUnionCase> cases)
    {
        return string.Join(ListSeparator, cases.Select(x => x.DisplayName));
    }

    /// <summary>
    /// A case as the union declares it.
    /// </summary>
    /// <param name="Type">The type the union stores for the case.</param>
    /// <param name="DeclaredType">The declared type, which differs from the stored type when the case is declared as a nullable value type.</param>
    /// <param name="IsDefault">A value indicating whether the case is what the default value of the union represents.</param>
    private readonly record struct CaseType(ITypeSymbol Type, ITypeSymbol DeclaredType, bool IsDefault);
}
