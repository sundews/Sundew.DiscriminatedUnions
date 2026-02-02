// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterfaceUnion.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tester;

[DiscriminatedUnion]
public partial interface InterfaceUnion
{
    public sealed partial record One() : InterfaceUnion;

    public sealed partial record Two() : InterfaceUnion;
}