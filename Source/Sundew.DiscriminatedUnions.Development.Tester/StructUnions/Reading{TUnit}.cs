// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reading{TUnit}.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tester.StructUnions;

/// <summary>
/// A generic struct union. The runtime does not allow a generic type to lay out its fields explicitly, so the cases that contain no references
/// and do not depend on the type parameter share a struct that is declared next to the union. The case that depends on it has a field of its own.
/// </summary>
/// <typeparam name="TUnit">The type of the unit.</typeparam>
public readonly partial struct Reading<TUnit>
    : ICase<Instant>,
      ICase<Interval>,
      ICase<Measured<TUnit>>;

public readonly record struct Instant(long Ticks);

public readonly record struct Interval(long From, long To);

public readonly record struct Measured<TUnit>(TUnit Unit, double Value);
