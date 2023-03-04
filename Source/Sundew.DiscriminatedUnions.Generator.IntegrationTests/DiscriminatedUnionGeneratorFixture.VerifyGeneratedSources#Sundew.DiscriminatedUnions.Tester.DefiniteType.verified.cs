//HintName: Sundew.DiscriminatedUnions.Tester.DefiniteType.cs
namespace Sundew.DiscriminatedUnions.Tester
{
    partial record DefiniteType
    {
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.NamedType))]
        public static global::Sundew.DiscriminatedUnions.Tester.DefiniteType NamedType(global::System.String name, global::System.String @namespace, global::System.String assemblyName) => new global::Sundew.DiscriminatedUnions.Tester.NamedType(name, @namespace, assemblyName);

        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.DefiniteArrayType))]
        public static global::Sundew.DiscriminatedUnions.Tester.DefiniteType DefiniteArrayType(global::Sundew.DiscriminatedUnions.Tester.DefiniteType elementType) => new global::Sundew.DiscriminatedUnions.Tester.DefiniteArrayType(elementType);
    }
}
