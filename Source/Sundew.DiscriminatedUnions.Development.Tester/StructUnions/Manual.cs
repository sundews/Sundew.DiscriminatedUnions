// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Manual.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tester.StructUnions;

/// <summary>
/// A struct union that opts out of generation, so that its members can be written by hand.
/// </summary>
[DiscriminatedUnion(disableGenerator: true)]
public readonly partial struct Manual
    : ICase<int>,
      ICase<long>;
