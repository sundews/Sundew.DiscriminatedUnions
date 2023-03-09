namespace Sundew.DiscriminatedUnions.Tester;

[DiscriminatedUnions.DiscriminatedUnion(generatorFeatures: GeneratorFeatures.Segregate)]
public abstract partial record DefiniteType(string Name, string Namespace, string? AssemblyName)
{
    public virtual string FullName => $"{this.Namespace}.{this.Name}";

    public string AssemblyQualifiedName => $"{this.FullName}, {this.AssemblyName}";
}

public sealed record NamedType(string Name, string Namespace, string? AssemblyName) : DefiniteType(Name, Namespace, AssemblyName);

public sealed record DefiniteArrayType(DefiniteType ElementType) : DefiniteType(ElementType.Name, ElementType.Namespace, ElementType.AssemblyName)
{
    public override string FullName => $"{this.Namespace}.{this.Name}";
}