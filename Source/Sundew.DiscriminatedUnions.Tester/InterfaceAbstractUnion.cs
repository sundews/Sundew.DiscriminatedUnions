namespace Sundew.DiscriminatedUnions.Tester;

[DiscriminatedUnion]
public partial interface IInterfaceUnion
{
}

[DiscriminatedUnion]
public abstract partial record InterfaceAbstractUnion : IInterfaceUnion
{
}

public sealed record RecordUnion(int Number) : InterfaceAbstractUnion;