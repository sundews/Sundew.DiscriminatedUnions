// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionSegregationProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Unions;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

/// <summary>
/// Finds what it takes to segregate a union: its cases with a name each, its type parameters and whether what is generated can be public.
/// </summary>
internal static class UnionSegregationProvider
{
    private const string ArraySuffix = "Array";
    private const string FallbackCaseName = "Case";
    private const string TypeArgumentsPrefix = "Of";
    private const string TypeArgumentsSeparator = "And";
    private const string UniqueSuffix = "_";
    private const string Unmanaged = "unmanaged";
    private const string Struct = "struct";
    private const string Class = "class";
    private const string NullableClass = "class?";
    private const string NotNull = "notnull";
    private const string Constructor = "new()";

    // A tuple is a positional pattern when it is written as one, so in a pattern it is written as the type it is.
    private static readonly SymbolDisplayFormat PatternTypeFormat = CodeAnalysisHelper.FullyQualifiedDisplayFormat.AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.ExpandValueTuple);

    /// <summary>
    /// Gets what is generated to segregate the specified union, if it asks for it with the generator features and has cases.
    /// The generated types are declared in the namespace of the union, so the union cannot be segregated
    /// when it or one of its cases is hidden within a type and cannot be referred to from there. The result then says which type that is.
    /// </summary>
    /// <param name="unionSymbol">The union.</param>
    /// <param name="caseTypes">The types of the cases of the union, in the order they are declared.</param>
    /// <param name="caseMatching">The way to find out which case a union is.</param>
    /// <param name="generatorFeatures">The generator features that the union declares.</param>
    /// <param name="compilation">The compilation, which knows how the case types relate to each other.</param>
    /// <param name="location">The location of the declaration of the union, which is where a problem is reported.</param>
    /// <returns>The segregation, the reason the union cannot be segregated or neither, if the union does not ask to be segregated.</returns>
    public static UnionSegregationResult GetSegregation(
        INamedTypeSymbol unionSymbol,
        IReadOnlyCollection<ITypeSymbol> caseTypes,
        UnionCaseMatching caseMatching,
        GeneratorFeatures generatorFeatures,
        Compilation compilation,
        Location location)
    {
        if (!generatorFeatures.HasFlag(GeneratorFeatures.Segregate) || caseTypes.Count == 0)
        {
            return default;
        }

        // A type parameter of the union that is named like one of a type the union is nested in hides that one, so the generated types could not have both.
        var typeParameters = GetTypeParameters(unionSymbol);
        var accessibility = caseTypes.Aggregate(UnionSymbolHelper.GetNamespaceAccessibility(unionSymbol), (current, caseType) => UnionSymbolHelper.NarrowAccessibility(current, UnionSymbolHelper.GetNamespaceAccessibility(caseType)));
        var inaccessibleType = UnionSymbolHelper.GetNamespaceAccessibility(unionSymbol) == Accessibility.NotApplicable || typeParameters.Select(x => x.Name).Distinct().Count() != typeParameters.Length
            ? unionSymbol
            : caseTypes.FirstOrDefault(x => UnionSymbolHelper.GetNamespaceAccessibility(x) == Accessibility.NotApplicable);
        if (inaccessibleType != null)
        {
            return new UnionSegregationResult(null, new UnionSegregationProblem(inaccessibleType.ToDisplayString(UnionSymbolHelper.DisplayNameFormat), DiagnosticLocation.From(location)));
        }

        var types = caseTypes.Select(x => x.WithNullableAnnotation(NullableAnnotation.NotAnnotated)).ToArray();
        var names = GetNames(types);
        var cases = types.Select((x, index) => new UnionSegregationCase(
            x.ToDisplayString(CodeAnalysisHelper.FullyQualifiedDisplayFormat),
            GetPatternType(x).ToDisplayString(PatternTypeFormat),
            x.ToDisplayString(UnionSymbolHelper.DisplayNameFormat),
            names[index])).ToImmutableArray();
        var segregation = new UnionSegregation(
            accessibility == Accessibility.Public,
            unionSymbol.ToDisplayString(CodeAnalysisHelper.FullyQualifiedDisplayFormat),
            typeParameters,
            caseMatching,
            cases,
            GetMatchOrder(types, cases, compilation));
        return new UnionSegregationResult(segregation, null);
    }

    private static ImmutableArray<UnionTypeParameter> GetTypeParameters(INamedTypeSymbol unionSymbol)
    {
        // The generated types are not nested in the types the union is nested in, so they need the type parameters of those as well.
        var types = new Stack<INamedTypeSymbol>();
        for (var type = unionSymbol; type != null; type = type.ContainingType)
        {
            types.Push(type);
        }

        return types.SelectMany(x => x.TypeParameters).Select(x => new UnionTypeParameter(x.Name, GetConstraints(x))).ToImmutableArray();
    }

    private static ImmutableArray<string> GetConstraints(ITypeParameterSymbol typeParameterSymbol)
    {
        var constraints = ImmutableArray.CreateBuilder<string>();
        if (typeParameterSymbol.HasUnmanagedTypeConstraint)
        {
            constraints.Add(Unmanaged);
        }
        else if (typeParameterSymbol.HasValueTypeConstraint)
        {
            constraints.Add(Struct);
        }
        else if (typeParameterSymbol.HasReferenceTypeConstraint)
        {
            constraints.Add(typeParameterSymbol.ReferenceTypeConstraintNullableAnnotation == NullableAnnotation.Annotated ? NullableClass : Class);
        }
        else if (typeParameterSymbol.HasNotNullConstraint)
        {
            constraints.Add(NotNull);
        }

        constraints.AddRange(typeParameterSymbol.ConstraintTypes.Select(x => x.ToDisplayString(CodeAnalysisHelper.FullyQualifiedDisplayFormat)));
        if (typeParameterSymbol.HasConstructorConstraint && !typeParameterSymbol.HasValueTypeConstraint)
        {
            constraints.Add(Constructor);
        }

        return constraints.ToImmutable();
    }

    private static string[] GetNames(IReadOnlyList<ITypeSymbol> caseTypes)
    {
        var simpleNames = caseTypes.Select(GetSimpleName).ToArray();

        // Cases that share a name, such as two constructions of one generic type, are told apart by their type arguments and, failing that, by their position.
        var qualifiedNames = caseTypes.Select((x, index) => simpleNames.Count(name => name == simpleNames[index]) > 1 ? GetQualifiedName(x) : simpleNames[index]).ToArray();
        var names = new string[qualifiedNames.Length];
        var takenNames = new HashSet<string>();
        for (var index = 0; index < qualifiedNames.Length; index++)
        {
            var qualifiedName = qualifiedNames[index];
            var name = qualifiedNames.Count(x => x == qualifiedName) > 1 ? qualifiedName + (index + 1).ToString(CultureInfo.InvariantCulture) : qualifiedName;
            while (!takenNames.Add(name))
            {
                name += UniqueSuffix;
            }

            names[index] = name;
        }

        return names;
    }

    private static string GetSimpleName(ITypeSymbol type)
    {
        return type switch
        {
            IArrayTypeSymbol arrayTypeSymbol => GetSimpleName(arrayTypeSymbol.ElementType) + ArraySuffix,
            INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullableTypeSymbol => GetSimpleName(nullableTypeSymbol.TypeArguments[0]),
            _ => string.IsNullOrEmpty(type.Name) ? FallbackCaseName : type.Name,
        };
    }

    private static string GetQualifiedName(ITypeSymbol type)
    {
        return type switch
        {
            IArrayTypeSymbol arrayTypeSymbol => GetQualifiedName(arrayTypeSymbol.ElementType) + ArraySuffix,
            INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullableTypeSymbol => GetQualifiedName(nullableTypeSymbol.TypeArguments[0]),
            INamedTypeSymbol { TypeArguments.Length: > 0 } namedTypeSymbol => namedTypeSymbol.Name + TypeArgumentsPrefix + string.Join(TypeArgumentsSeparator, namedTypeSymbol.TypeArguments.Select(GetQualifiedName)),
            _ => GetSimpleName(type),
        };
    }

    private static ITypeSymbol GetPatternType(ITypeSymbol type)
    {
        // A nullable value type cannot be written in a pattern. The type it wraps matches the values it has.
        return type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullableTypeSymbol ? nullableTypeSymbol.TypeArguments[0] : type;
    }

    private static ImmutableArray<int> GetMatchOrder(IReadOnlyList<ITypeSymbol> caseTypes, ImmutableArray<UnionSegregationCase> cases, Compilation compilation)
    {
        var patternTypes = caseTypes.Select(GetPatternType).ToArray();
        var remaining = new List<int>();
        var patternTypeNames = new HashSet<string>();
        for (var index = 0; index < cases.Length; index++)
        {
            // Cases that are written the same in a pattern cannot be told apart, so the first of them gets the items.
            if (patternTypeNames.Add(cases[index].PatternTypeName))
            {
                remaining.Add(index);
            }
        }

        // A type pattern also matches the types that derive from its type, so the most specific types are matched first.
        // The compiler insists on that order: a pattern that an earlier one already handles is an error.
        var matchOrder = ImmutableArray.CreateBuilder<int>(remaining.Count);
        while (remaining.Count > 0)
        {
            var next = remaining.Where(x => !remaining.Any(y => y != x && IsMoreSpecific(patternTypes[y], patternTypes[x], compilation))).DefaultIfEmpty(remaining[0]).First();
            matchOrder.Add(next);
            remaining.Remove(next);
        }

        return matchOrder.ToImmutable();
    }

    private static bool IsMoreSpecific(ITypeSymbol type, ITypeSymbol otherType, Compilation compilation)
    {
        if (ConvertsTo(type, otherType, compilation))
        {
            return true;
        }

        // A type parameter can be any type, so where nothing else is known the other types are matched before it.
        return otherType.TypeKind == TypeKind.TypeParameter && type.TypeKind != TypeKind.TypeParameter && !ConvertsTo(otherType, type, compilation);
    }

    private static bool ConvertsTo(ITypeSymbol type, ITypeSymbol otherType, Compilation compilation)
    {
        var conversion = compilation.ClassifyConversion(type, otherType);
        return conversion.Exists && conversion.IsImplicit && !conversion.IsIdentity && (conversion.IsReference || conversion.IsBoxing);
    }
}
