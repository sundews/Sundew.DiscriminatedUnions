//HintName: Sundew.DiscriminatedUnions.Tester.DefiniteTypeSegregation.cs
namespace Sundew.DiscriminatedUnions.Tester
{
    public sealed class DefiniteTypeSegregation
    {
        internal DefiniteTypeSegregation(System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.NamedType> namedTypes, System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.DefiniteArrayType> definiteArrayTypes)
        {
            this.NamedTypes = namedTypes;
            this.DefiniteArrayTypes = definiteArrayTypes;
        }

        public System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.NamedType> NamedTypes { get; }

        public System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.DefiniteArrayType> DefiniteArrayTypes { get; }
    }
}