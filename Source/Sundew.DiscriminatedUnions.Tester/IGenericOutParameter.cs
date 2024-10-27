namespace Sundew.DiscriminatedUnions.Tester;
[DiscriminatedUnion(generatorFeatures: GeneratorFeatures.Segregate)]
public partial interface IGenericOutParameter<out T>
    where T : notnull
{

}


public sealed record GenericOutParameter<T>(T Value) : IGenericOutParameter<T>
    where T : notnull;

public sealed record SpecialGenericOutParameter<T>(T Value) : IGenericOutParameter<T>
    where T : notnull;