// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NestedGenericUnion.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tester;

using System.Collections.Generic;
using System.Linq;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract partial record NestedGenericUnion<TItem>
{
    public sealed partial record Target(TItem Item) : NestedGenericUnion<TItem>;

    public sealed partial record TargetList(List<TItem> Item) : NestedGenericUnion<TItem>;
}

public class NestedTest
{
    public void Test()
    {
        var nestedGenericUnion = NestedGenericUnion<string>._Target("Test");
        var result2 = nestedGenericUnion switch
        {
            NestedGenericUnion<string>.Target target => target.Item,
            NestedGenericUnion<string>.TargetList targetList => targetList.Item.First(),
        };
    }
}