// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Measurement.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tester.StructUnions;

/// <summary>
/// A struct union whose cases have the same name and only differ by their type arguments.
/// When it is segregated, the type arguments are what tells the lists of those cases apart.
/// </summary>
[DiscriminatedUnion(GeneratorFeatures.Segregate)]
public readonly partial struct Measurement
    : IDefaultCase<Unknown>,
      ICase<Quantity<int>>,
      ICase<Quantity<double>>;

public readonly record struct Unknown;

public readonly record struct Quantity<TValue>(TValue Value)
    where TValue : struct;
