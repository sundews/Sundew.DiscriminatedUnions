// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionExtensions.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Shared;

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;

/// <summary>
/// Helpers for analyzing discriminated unions.
/// </summary>
public static class DiscriminatedUnionExtensions
{
    /// <summary>
    /// Determines whether [is discriminated union] [the specified union type].
    /// </summary>
    /// <param name="unionType">Type of the union.</param>
    /// <returns>
    ///   <c>true</c> if [is discriminated union] [the specified union type]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsDiscriminatedUnion([NotNullWhen(true)] this ITypeSymbol? unionType)
    {
        if (unionType == null)
        {
            return false;
        }

        // A closed type and a union are checked by the compiler. The attribute then only configures the source generator.
        return !unionType.IsClosed && !unionType.IsUnion && HasDiscriminatedUnionAttribute(unionType);
    }

    /// <summary>
    /// Determines whether [is discriminated union] [the specified union type].
    /// </summary>
    /// <param name="unionType">Type of the union.</param>
    /// <returns>
    ///   <c>true</c> if [is discriminated union] [the specified union type]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsDiscriminatedUnionLike([NotNullWhen(true)] this INamedTypeSymbol? unionType)
    {
        if (unionType == null)
        {
            return false;
        }

        if (unionType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
        {
            return unionType.TypeArguments.Single().IsDiscriminatedUnion();
        }

        return unionType.IsDiscriminatedUnion();
    }

    /// <summary>
    /// Determines whether the type is generic.
    /// </summary>
    /// <param name="namedTypeSymbol">The named type symbol.</param>
    /// <returns><c>true</c>, if this is generic and has type parameters, otherwise <c>false</c>.</returns>
    public static bool IsTypeGeneric(this INamedTypeSymbol namedTypeSymbol)
    {
        return namedTypeSymbol.IsGenericType | namedTypeSymbol.IsUnboundGenericType;
    }

    /// <summary>
    /// Determines whether the type is generic and has type parameters.
    /// </summary>
    /// <param name="namedTypeSymbol">The named type symbol.</param>
    /// <returns><c>true</c>, if this is generic and has type parameters, otherwise <c>false</c>.</returns>
    public static bool IsTypeGenericWithTypeParameters(this INamedTypeSymbol namedTypeSymbol)
    {
        return IsTypeGeneric(namedTypeSymbol) && namedTypeSymbol.TypeParameters.Length > 0;
    }

    private static bool HasDiscriminatedUnionAttribute(ITypeSymbol unionType)
    {
        // This runs for every type an analyzer looks at, so it neither creates a delegate nor a string per attribute.
        foreach (var attribute in unionType.GetAttributes())
        {
            if (attribute.AttributeClass.IsDiscriminatedUnionAttribute())
            {
                return true;
            }
        }

        return false;
    }
}
