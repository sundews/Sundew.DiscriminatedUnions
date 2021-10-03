// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImmutableHashSetExtesions.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Analyzer
{
    using System;
    using System.Collections.Immutable;

    internal static class ImmutableHashSetExtesions
    {
        public static ImmutableHashSet<TItem> Add<TItem>(this ImmutableHashSet<TItem> immutableHashSet, TItem item, Action<ImmutableHashSet<TItem>, TItem> onAdded)
        {
            var hashSet = immutableHashSet.Add(item);
            if (!ReferenceEquals(immutableHashSet, hashSet))
            {
                onAdded(hashSet, item);
            }

            return hashSet;
        }
    }
}