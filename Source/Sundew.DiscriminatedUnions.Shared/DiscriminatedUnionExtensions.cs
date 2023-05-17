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

        return unionType.GetAttributes().Any(attribute =>
        {
            var containingClass = attribute.AttributeClass?.ToDisplayString();
            return containingClass == typeof(Sundew.DiscriminatedUnions.DiscriminatedUnion).FullName;
        });
    }
}