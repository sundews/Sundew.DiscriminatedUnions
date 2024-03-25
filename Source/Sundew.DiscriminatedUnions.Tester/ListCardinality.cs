namespace Sundew.DiscriminatedUnions.Tester;

using System.Collections;
using System.Collections.Generic;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract partial class ListCardinality<TItem>
{
}

public sealed class Empty<TItem> : ListCardinality<TItem>
{
}

public sealed class Single<TItem> : ListCardinality<TItem>
{
    internal Single(TItem item)
    {
        this.Item = item;
    }

    public TItem Item { get; }
}

public sealed class Multiple<TItem> : ListCardinality<TItem>, IEnumerable<TItem>
{
    public Multiple(IEnumerable<TItem> items)
    {
        this.Items = items;
    }

    public IEnumerable<TItem> Items { get; }

    public IEnumerator<TItem> GetEnumerator()
    {
        return this.Items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}