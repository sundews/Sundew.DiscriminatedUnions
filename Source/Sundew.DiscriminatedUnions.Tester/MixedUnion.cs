namespace Sundew.DiscriminatedUnions.Tester;

using System.Collections.Generic;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract partial record MixedUnion
{
    public sealed record Case1(string Info, bool IsValid = false, Status Status = Status.On, int Number = 3) : MixedUnion;
}

public sealed record Case2(int? Info = 4, List<string?>? OptionalList = null, List<int?>? OptionalInts = null) : MixedUnion;

public sealed record Case3 : MixedUnion;

public enum Status
{
    Off,
    On,
}