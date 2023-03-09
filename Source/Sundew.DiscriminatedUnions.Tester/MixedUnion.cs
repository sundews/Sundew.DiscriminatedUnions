namespace Sundew.DiscriminatedUnions.Tester;

using System.Collections.Generic;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract partial record MixedUnion
{
    public sealed record Case1(string Info) : MixedUnion;
}

public sealed record Case2(int? Info = 4, List<string?>? OptionalList = null, List<int?>? OptionalInts = null) : MixedUnion;