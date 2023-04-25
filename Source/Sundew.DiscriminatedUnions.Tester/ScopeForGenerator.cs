namespace Sundew.DiscriminatedUnions.Tester;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract partial record ScopeForGenerator
{
    internal sealed record AutoScopeForGenerator : ScopeForGenerator;

    internal sealed record SingleInstancePerFuncResultScopeForGenerator(string Method) : ScopeForGenerator;
}