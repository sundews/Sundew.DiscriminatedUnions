// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInterfaceUnion.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tester;

[DiscriminatedUnion]
public partial interface IInterfaceUnion
{
}

[DiscriminatedUnion]
public abstract partial record InterfaceAbstractUnion : IInterfaceUnion
{
}

public sealed record RecordUnion(int Number) : InterfaceAbstractUnion;