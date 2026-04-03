// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDoubleInterfaceUnion.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tester;

[DiscriminatedUnion]
public partial interface IDoubleInterfaceUnion : IInterface
{
    public sealed partial record DoubleInterfaceUnion1() : IDoubleInterfaceUnion;

    public sealed partial record DoubleInterfaceUnion2() : IDoubleInterfaceUnion;
}

[DiscriminatedUnion]
public partial interface IInterface
{
}