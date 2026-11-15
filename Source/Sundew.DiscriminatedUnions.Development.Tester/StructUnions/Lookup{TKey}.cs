// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Lookup{TKey}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tester.StructUnions;

/// <summary>
/// A generic struct union with a default case and a single reference case, which gets a field of its own.
/// </summary>
/// <typeparam name="TKey">The key type.</typeparam>
public readonly partial struct Lookup<TKey>
    : IDefaultCase<Missing>,
      ICase<Found<TKey>>,
      ICase<string>;

public readonly record struct Missing;

public readonly record struct Found<TKey>(TKey Key, int Index);
