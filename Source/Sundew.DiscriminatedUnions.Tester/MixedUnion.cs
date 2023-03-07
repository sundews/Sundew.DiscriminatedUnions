namespace Sundew.DiscriminatedUnions.Tester;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract partial record MixedUnion
{
    public sealed record Case1(string Info) : MixedUnion;
}

public sealed record Case2(string Info) : MixedUnion;