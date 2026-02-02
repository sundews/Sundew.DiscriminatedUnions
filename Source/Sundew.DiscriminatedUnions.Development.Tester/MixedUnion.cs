// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MixedUnion.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tester;

using System.Collections.Generic;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract partial record MixedUnion
{
    public sealed partial record Case1(string Info, bool IsValid = false, Status Status = Status.On, int Number = 3) : MixedUnion;
}

public sealed partial record Case2(int? Info = 4, List<string?>? OptionalList = null, List<int?>? OptionalInts = null) : MixedUnion;

public sealed partial record Case3 : MixedUnion;

public enum Status
{
    Off,
    On,
}