// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDimensionalUnion1.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tester;

[DiscriminatedUnion]
public partial interface IDimensionalUnion1
{
}

[DiscriminatedUnion]
public partial interface IDimensionalUnion2
{
}

public sealed partial record TwoDimensionalUnion(int Number) : IDimensionalUnion1, IDimensionalUnion2;