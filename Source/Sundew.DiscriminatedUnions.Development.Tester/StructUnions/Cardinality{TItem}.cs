// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cardinality{TItem}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tester.StructUnions;

using System.Collections.Generic;

/// <summary>
/// A generic struct union with a default case. The factory members have the same names as the cases,
/// so the generated code has to refer to the cases by their fully qualified names.
/// It asks to be segregated, which takes its type parameter along to the generated types.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
[DiscriminatedUnion(GeneratorFeatures.Segregate)]
public readonly partial struct Cardinality<TItem>
    : IDefaultCase<Empty<TItem>>,
      ICase<Single<TItem>>,
      ICase<Multiple<TItem>>
{
    public static Cardinality<TItem> Empty => default;

    public static Cardinality<TItem> Single(TItem item)
    {
        return new Cardinality<TItem>(new global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Single<TItem>(item));
    }

    public static Cardinality<TItem> Multiple(IEnumerable<TItem> items)
    {
        return new Cardinality<TItem>(new global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Multiple<TItem>(items));
    }
}

public readonly record struct Empty<TItem>;

public readonly record struct Single<TItem>(TItem Item);

public readonly record struct Multiple<TItem>(IEnumerable<TItem> Items);
