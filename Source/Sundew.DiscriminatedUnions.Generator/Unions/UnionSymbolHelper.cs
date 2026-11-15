// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionSymbolHelper.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Unions;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sundew.DiscriminatedUnions.Shared;

/// <summary>
/// Reads what a union declares about how it should be generated.
/// </summary>
internal static class UnionSymbolHelper
{
    private const string CasesName = "Cases";
    private const string EqualsName = "Equals";
    private const string GetHashCodeName = "GetHashCode";
    private const string ToStringName = "ToString";
    private const char FeatureSeparator = '|';
    private const char MemberAccessSeparator = '.';

    /// <summary>
    /// Gets the names of the members every type has, which a generated member of the same name would collide with.
    /// </summary>
    public static ImmutableArray<string> ObjectMemberNames { get; } = ImmutableArray.Create("Equals", "GetHashCode", "GetType", "ToString", "MemberwiseClone", "ReferenceEquals", "Finalize");

    /// <summary>
    /// Gets the format for the name of a type as it is written in documentation and diagnostics.
    /// </summary>
    public static SymbolDisplayFormat DisplayNameFormat { get; } = new(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers | SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

    /// <summary>
    /// Gets the discriminated union attribute that the type declares.
    /// </summary>
    /// <param name="typeSymbol">The type.</param>
    /// <returns>The attribute or <c>null</c>, if the type does not declare it.</returns>
    public static AttributeData? GetDiscriminatedUnionAttribute(ITypeSymbol typeSymbol)
    {
        foreach (var attribute in typeSymbol.GetAttributes())
        {
            if (attribute.AttributeClass.IsDiscriminatedUnionAttribute())
            {
                return attribute;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets a value indicating whether the discriminated union attribute opts the union out of generation.
    /// </summary>
    /// <param name="attributeData">The discriminated union attribute, if the union declares it.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c>, if the generator is disabled for the union, otherwise <c>false</c>.</returns>
    public static bool IsGeneratorDisabled(AttributeData? attributeData, CancellationToken cancellationToken)
    {
        return GetArgument(attributeData) switch
        {
            bool isGeneratorDisabled => isGeneratorDisabled,
            null => GetArgumentSyntax(attributeData, cancellationToken)?.Expression.IsKind(SyntaxKind.TrueLiteralExpression) == true,
            _ => false,
        };
    }

    /// <summary>
    /// Gets the generator features that the discriminated union attribute declares.
    /// </summary>
    /// <param name="attributeData">The discriminated union attribute, if the union declares it.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The generator features.</returns>
    public static GeneratorFeatures GetGeneratorFeatures(AttributeData? attributeData, CancellationToken cancellationToken)
    {
        var argument = GetArgument(attributeData);
        if (argument != null)
        {
            return argument is int generatorFeatures ? (GeneratorFeatures)generatorFeatures : GeneratorFeatures.None;
        }

        var features = GeneratorFeatures.None;
        foreach (var name in GetArgumentSyntax(attributeData, cancellationToken)?.Expression.ToString().Split(FeatureSeparator) ?? [])
        {
            if (Enum.TryParse<GeneratorFeatures>(name.Substring(name.LastIndexOf(MemberAccessSeparator) + 1).Trim(), out var feature))
            {
                features |= feature;
            }
        }

        return features;
    }

    /// <summary>
    /// Gets the members that can be generated for the union, which are those the union does not declare itself.
    /// </summary>
    /// <param name="typeSymbol">The union.</param>
    /// <returns>The members to generate.</returns>
    public static GeneratedMembers GetGeneratedMembers(INamedTypeSymbol typeSymbol)
    {
        var generatedMembers = GeneratedMembers.All;
        foreach (var member in typeSymbol.GetMembers().Where(x => !x.IsImplicitlyDeclared))
        {
            switch (member)
            {
                case { Name: CasesName }:
                    generatedMembers &= ~GeneratedMembers.Cases;
                    break;
                case IMethodSymbol { Name: ToStringName, Parameters.Length: 0 }:
                    generatedMembers &= ~GeneratedMembers.ToString;
                    break;
                case IMethodSymbol { Name: EqualsName, Parameters.Length: 1 }:
                case IMethodSymbol { Name: GetHashCodeName, Parameters.Length: 0 }:
                case IMethodSymbol { Name: WellKnownMemberNames.EqualityOperatorName or WellKnownMemberNames.InequalityOperatorName }:
                    generatedMembers &= ~GeneratedMembers.Equality;
                    break;
            }
        }

        return generatedMembers;
    }

    /// <summary>
    /// Gets the cases of the specified closed type: the types that derive from it. Where one of those is a closed type itself,
    /// the types that derive from that one take its place, because they are what a switch has to handle.
    /// </summary>
    /// <param name="closedTypeSymbol">The closed type.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The case types, in the order the compiler lists the derived types.</returns>
    public static ImmutableArray<ITypeSymbol> GetClosedTypeCases(INamedTypeSymbol closedTypeSymbol, CancellationToken cancellationToken)
    {
        // A derived type that fixes the type arguments of a generic closed type is not a case of every construction of it, so it cannot be referred to in general.
        return LanguageUnionExtensions.GetClosedTypeCases(closedTypeSymbol, cancellationToken).Where(x => DerivesFrom(x, closedTypeSymbol)).ToImmutableArray();
    }

    /// <summary>
    /// Gets how accessible the specified type is to a type that is declared in a namespace of the same assembly.
    /// </summary>
    /// <param name="typeSymbol">The type.</param>
    /// <returns>Public, internal, or not applicable when the type or a type it is built from is hidden within a type or cannot be referred to at all.</returns>
    public static Accessibility GetNamespaceAccessibility(ITypeSymbol typeSymbol)
    {
        switch (typeSymbol)
        {
            case ITypeParameterSymbol:
                return Accessibility.Public;
            case IArrayTypeSymbol arrayTypeSymbol:
                return GetNamespaceAccessibility(arrayTypeSymbol.ElementType);
            case INamedTypeSymbol { TypeKind: not TypeKind.Error } namedTypeSymbol:
                var accessibility = Accessibility.Public;
                for (var type = namedTypeSymbol; type != null; type = type.ContainingType)
                {
                    accessibility = NarrowAccessibility(accessibility, type.DeclaredAccessibility switch
                    {
                        Accessibility.Public => Accessibility.Public,
                        Accessibility.Internal or Accessibility.ProtectedOrInternal => Accessibility.Internal,
                        _ => Accessibility.NotApplicable,
                    });
                    accessibility = type.TypeArguments.Aggregate(accessibility, (current, typeArgument) => NarrowAccessibility(current, GetNamespaceAccessibility(typeArgument)));
                }

                return accessibility;
            default:
                // What remains, such as a pointer, cannot be a type argument, so generated code has no way to keep it in a list.
                return Accessibility.NotApplicable;
        }
    }

    /// <summary>
    /// Gets the narrower of two accessibilities, where not applicable is the narrowest.
    /// </summary>
    /// <param name="accessibility">The accessibility.</param>
    /// <param name="other">The other accessibility.</param>
    /// <returns>Public, internal or not applicable.</returns>
    public static Accessibility NarrowAccessibility(Accessibility accessibility, Accessibility other)
    {
        if (accessibility == Accessibility.NotApplicable || other == Accessibility.NotApplicable)
        {
            return Accessibility.NotApplicable;
        }

        return accessibility == Accessibility.Internal || other == Accessibility.Internal ? Accessibility.Internal : Accessibility.Public;
    }

    private static bool DerivesFrom(ITypeSymbol typeSymbol, INamedTypeSymbol baseTypeSymbol)
    {
        for (var baseType = typeSymbol.BaseType; baseType != null; baseType = baseType.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(baseType, baseTypeSymbol))
            {
                return true;
            }
        }

        return false;
    }

    private static object? GetArgument(AttributeData? attributeData)
    {
        // The attribute takes at most one argument: the generator features or whether the generator is disabled.
        // An argument that cannot be bound, for example while it contains an error, has no value. What a union declares
        // should not depend on that, so the callers read the argument from the syntax when there is no value.
        return attributeData is { ConstructorArguments.Length: 1 } ? attributeData.ConstructorArguments[0].Value : null;
    }

    private static AttributeArgumentSyntax? GetArgumentSyntax(AttributeData? attributeData, CancellationToken cancellationToken)
    {
        return (attributeData?.ApplicationSyntaxReference?.GetSyntax(cancellationToken) as AttributeSyntax)?.ArgumentList?.Arguments.FirstOrDefault();
    }
}
