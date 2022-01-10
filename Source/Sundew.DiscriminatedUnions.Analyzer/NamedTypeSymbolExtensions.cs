// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamedTypeSymbolExtensions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer;

using Microsoft.CodeAnalysis;
using System.Collections.Generic;

/// <summary>
/// Extends <see cref="INamedTypeSymbol"/> with easy to use methods.
/// </summary>
public static class NamedTypeSymbolExtensions
{
    /// <summary>
    /// Enumerates the type hierarchy.
    /// </summary>
    /// <param name="discriminatedUnionType">The discriminated union type.</param>
    /// <returns>An enumerable of all base types.</returns>
    public static IEnumerable<INamedTypeSymbol> EnumerateBaseTypes(this INamedTypeSymbol? discriminatedUnionType)
    {
        if (discriminatedUnionType == null)
        {
            yield break;
        }

        discriminatedUnionType = discriminatedUnionType.BaseType;
        while (discriminatedUnionType != null)
        {
            yield return discriminatedUnionType;
            discriminatedUnionType = discriminatedUnionType.BaseType;
        }
    }
}