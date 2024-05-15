namespace Sundew.DiscriminatedUnions.Tester;

using System.Collections.Generic;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract partial record NestedGenericUnion<TItem>
{
    public sealed record Target(TItem Item) : NestedGenericUnion<TItem>;

    public sealed record TargetList(List<TItem> Item) : NestedGenericUnion<TItem>;
}