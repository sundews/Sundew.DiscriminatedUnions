// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionFactoryProvider.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Unions;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

/// <summary>
/// Finds the factories that are generated for a union.
/// </summary>
internal static class UnionFactoryProvider
{
    private const string ObsoleteAttributeName = nameof(ObsoleteAttribute);
    private const string SetsRequiredMembersAttributeName = "SetsRequiredMembersAttribute";
    private const string SignatureSeparator = ",";
    private const string NestedCasePrefix = "_";

    // The members that a struct union or a union declaration has or gets generated.
    private static readonly string[] UnionMemberNames = ["Cases", "Value", "HasValue", "TryGetValue"];

    // The member that a closed type gets generated.
    private static readonly string[] ClosedTypeMemberNames = ["Cases"];

    /// <summary>
    /// Gets the factories for the specified struct union or union declaration.
    /// A case gets a factory when it is a class or a struct that is declared in the assembly of the union and has a constructor the union can call.
    /// It gets none when the union already has a member of its name, because a union keeps the members it declares.
    /// Cases that share a name, such as two constructions of one generic type, get factories that overload each other when their parameters tell them apart.
    /// </summary>
    /// <param name="unionSymbol">The union.</param>
    /// <param name="caseTypes">The types of the cases of the union.</param>
    /// <param name="boxesCases">A value indicating whether the union stores its cases as objects, so that creating it from a struct allocates.</param>
    /// <returns>The factories, in the order of the cases.</returns>
    public static ImmutableArray<UnionFactory> GetFactories(INamedTypeSymbol unionSymbol, IReadOnlyCollection<ITypeSymbol> caseTypes, bool boxesCases)
    {
        return GetFactories(unionSymbol, caseTypes, UnionMemberNames, boxesCases);
    }

    /// <summary>
    /// Gets the factories for the specified closed type, for which a case is the union and is returned as it is.
    /// </summary>
    /// <param name="closedTypeSymbol">The closed type.</param>
    /// <param name="caseTypes">The types of the cases of the closed type.</param>
    /// <returns>The factories, in the order of the cases.</returns>
    public static ImmutableArray<UnionFactory> GetClosedTypeFactories(INamedTypeSymbol closedTypeSymbol, IReadOnlyCollection<ITypeSymbol> caseTypes)
    {
        return GetFactories(closedTypeSymbol, caseTypes, ClosedTypeMemberNames, true);
    }

    private static ImmutableArray<UnionFactory> GetFactories(INamedTypeSymbol unionSymbol, IReadOnlyCollection<ITypeSymbol> caseTypes, IEnumerable<string> generatedMemberNames, bool boxesCases)
    {
        var takenNames = new HashSet<string>(UnionSymbolHelper.ObjectMemberNames) { unionSymbol.Name };
        takenNames.UnionWith(generatedMemberNames);
        takenNames.UnionWith(unionSymbol.GetMembers().Select(x => x.Name));
        takenNames.UnionWith(unionSymbol.TypeParameters.Select(x => x.Name));
        var factories = ImmutableArray.CreateBuilder<UnionFactory>();
        foreach (var caseType in caseTypes)
        {
            if (caseType is not INamedTypeSymbol { TypeKind: TypeKind.Class or TypeKind.Struct, IsAbstract: false, IsTupleType: false } namedTypeSymbol
                || !SymbolEqualityComparer.Default.Equals(namedTypeSymbol.ContainingAssembly, unionSymbol.ContainingAssembly))
            {
                continue;
            }

            // Like the factories of class unions, the factory mirrors the constructor that takes the most parameters.
            var hasRequiredMembers = HasRequiredMembers(namedTypeSymbol);
            var constructor = namedTypeSymbol.InstanceConstructors.Where(x => CanBeCalled(x, hasRequiredMembers)).OrderByDescending(x => x.Parameters.Length).FirstOrDefault();
            var name = GetName(namedTypeSymbol, unionSymbol);
            var accessibility = GetAccessibility(namedTypeSymbol);
            if (constructor == null || accessibility == Accessibility.NotApplicable || takenNames.Contains(name))
            {
                continue;
            }

            var type = namedTypeSymbol.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
            factories.Add(new UnionFactory(
                name,
                type.ToDisplayString(CodeAnalysisHelper.FullyQualifiedDisplayFormat),
                type.ToDisplayString(UnionSymbolHelper.DisplayNameFormat),
                accessibility == Accessibility.Public && constructor.DeclaredAccessibility == Accessibility.Public,
                constructor.Parameters.Select(x => x.GetParameter()).ToImmutableArray(),
                constructor.Parameters.LastOrDefault()?.IsParams == true,
                boxesCases || namedTypeSymbol.IsReferenceType));
        }

        return factories.Where(x => CanBeDeclared(x, factories)).ToImmutableArray();
    }

    private static string GetName(INamedTypeSymbol caseType, INamedTypeSymbol unionSymbol)
    {
        // A case that is nested in the type it is a case of already is the member of its name there, so its factory gets a prefix, as it does for class unions.
        // For closed types that includes the closed types between the case and the union, so that the factory has the same name in all of them.
        var containingType = caseType.ContainingType?.OriginalDefinition;
        var isNested = SymbolEqualityComparer.Default.Equals(containingType, unionSymbol.OriginalDefinition);
        for (var baseType = caseType.BaseType; !isNested && baseType is { IsClosed: true }; baseType = baseType.BaseType)
        {
            isNested = SymbolEqualityComparer.Default.Equals(containingType, baseType.OriginalDefinition);
        }

        return isNested ? NestedCasePrefix + caseType.Name : caseType.Name.AvoidKeywordCollision();
    }

    private static Accessibility GetAccessibility(INamedTypeSymbol caseType)
    {
        // A factory does not make a case available where the case itself is not: it is internal for an internal case, and a case that is hidden within a type gets none.
        var accessibility = Accessibility.Public;
        for (var type = caseType; type != null; type = type.ContainingType)
        {
            switch (type.DeclaredAccessibility)
            {
                case Accessibility.Public:
                    break;
                case Accessibility.Internal or Accessibility.ProtectedOrInternal:
                    accessibility = Accessibility.Internal;
                    break;
                default:
                    return Accessibility.NotApplicable;
            }
        }

        return accessibility;
    }

    private static bool CanBeDeclared(UnionFactory factory, IReadOnlyCollection<UnionFactory> factories)
    {
        // Factories of the same name have to be methods with different parameters. A factory without parameters is a property, which cannot share its name.
        var sameName = factories.Where(x => x.Name == factory.Name).ToArray();
        return sameName.Length == 1
               || (sameName.All(x => x.Parameters.Count > 0) && sameName.Select(GetSignature).Distinct().Count() == sameName.Length);
    }

    private static string GetSignature(UnionFactory factory)
    {
        return string.Join(SignatureSeparator, factory.Parameters.Select(x => x.TypeName));
    }

    private static bool CanBeCalled(IMethodSymbol constructor, bool hasRequiredMembers)
    {
        // A record has a constructor that copies another instance, which is not a way to create a new one.
        var isCopyConstructor = constructor.ContainingType.IsRecord
                                && constructor.Parameters.Length == 1
                                && SymbolEqualityComparer.Default.Equals(constructor.Parameters[0].Type, constructor.ContainingType);
        return constructor.DeclaredAccessibility is Accessibility.Public or Accessibility.Internal or Accessibility.ProtectedOrInternal
               && !isCopyConstructor
               && constructor.Parameters.All(x => x.RefKind is RefKind.None or RefKind.In && x.Type.TypeKind is not (TypeKind.Pointer or TypeKind.FunctionPointer))
               && !HasAttribute(constructor, static x => x is { Name: ObsoleteAttributeName, ContainingType: null, ContainingNamespace: { Name: nameof(System), ContainingNamespace.IsGlobalNamespace: true } })
               && (!hasRequiredMembers || HasAttribute(constructor, static x => x is { Name: SetsRequiredMembersAttributeName, ContainingType: null, ContainingNamespace: { Name: nameof(System.Diagnostics.CodeAnalysis), ContainingNamespace: { Name: nameof(System.Diagnostics), ContainingNamespace: { Name: nameof(System), ContainingNamespace.IsGlobalNamespace: true } } } }));
    }

    private static bool HasRequiredMembers(INamedTypeSymbol typeSymbol)
    {
        // A required member has to be set where the instance is created, which a factory cannot do.
        for (var type = typeSymbol; type != null; type = type.BaseType)
        {
            if (type.GetMembers().Any(x => x is IPropertySymbol { IsRequired: true } or IFieldSymbol { IsRequired: true }))
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasAttribute(ISymbol symbol, Func<INamedTypeSymbol?, bool> isAttribute)
    {
        // The attribute is recognized by its name and namespaces, which creates no string for each attribute the way its display name would.
        foreach (var attribute in symbol.GetAttributes())
        {
            if (isAttribute(attribute.AttributeClass))
            {
                return true;
            }
        }

        return false;
    }
}