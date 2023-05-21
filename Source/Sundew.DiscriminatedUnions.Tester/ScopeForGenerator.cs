namespace Sundew.DiscriminatedUnions.Tester;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract partial record ScopeForGenerator
{
    internal sealed record Auto : ScopeForGenerator;

    internal sealed record SingleInstancePerFuncResult(string Method) : ScopeForGenerator;
}