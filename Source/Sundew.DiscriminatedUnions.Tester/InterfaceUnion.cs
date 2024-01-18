namespace Sundew.DiscriminatedUnions.Tester;

[DiscriminatedUnion]
public partial interface InterfaceUnion
{
    public sealed record One() : InterfaceUnion;

    public sealed record Two() : InterfaceUnion;
}