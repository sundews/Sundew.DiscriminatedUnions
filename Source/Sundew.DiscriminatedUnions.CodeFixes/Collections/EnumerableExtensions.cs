// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.CodeFixes.Collections;

using System.Collections.Generic;

internal static class EnumerableExtensions
{
    public static IEnumerable<(TItem? Previous, TItem Current)> Pair<TItem>(this IEnumerable<TItem> enumerable)
    {
        var previous = default(TItem);
        foreach (var item in enumerable)
        {
            yield return (previous, item);
            previous = item;
        }
    }
}