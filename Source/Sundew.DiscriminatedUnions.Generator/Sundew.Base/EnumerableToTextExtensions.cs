// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableToTextExtensions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Base;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

/// <summary>
/// Extends IEnumerable with <see cref="StringBuilder"/> aggregate methods.
/// </summary>
public static class EnumerableToTextExtensions
{
    /// <summary>
    /// Aggregates to string builder.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="stringBuilder">The string builder.</param>
    /// <param name="aggregateAction">The aggregate action.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result of the result function.
    /// </returns>
    public static StringBuilder JoinToStringBuilder<TItem>(
        this IEnumerable<TItem> enumerable,
        StringBuilder stringBuilder,
        Action<StringBuilder, TItem> aggregateAction,
        string separator)
    {
        return enumerable.Aggregate(
            (stringBuilder, isSuccessive: false),
            (builderPair, item) =>
            {
                aggregateAction.Invoke(builderPair.stringBuilder, item);
                builderPair.stringBuilder.Append(separator);
                return builderPair with { isSuccessive = true };
            },
            builderPair => RemoveSeparatorAtEnd(builderPair, separator));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static StringBuilder RemoveSeparatorAtEnd((StringBuilder StringBuilder, bool IsSuccessive) builderPair, string separator)
    {
        return builderPair.IsSuccessive ? builderPair.StringBuilder.Remove(builderPair.StringBuilder.Length - separator.Length, separator.Length) : builderPair.StringBuilder;
    }
}