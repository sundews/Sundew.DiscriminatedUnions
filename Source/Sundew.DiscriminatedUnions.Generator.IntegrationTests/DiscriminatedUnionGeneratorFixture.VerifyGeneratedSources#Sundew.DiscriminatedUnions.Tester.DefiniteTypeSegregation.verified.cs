//HintName: Sundew.DiscriminatedUnions.Tester.DefiniteTypeSegregation.cs
namespace Sundew.DiscriminatedUnions.Tester
{
    /// <summary>
    /// Contains individual lists of the different cases of the discriminated union DefiniteType.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "3.0.0.0")]
    public sealed class DefiniteTypeSegregation
    {
        internal DefiniteTypeSegregation(System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.NamedType> namedTypes, System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.DefiniteArrayType> definiteArrayTypes)
        {
            this.NamedTypes = namedTypes;
            this.DefiniteArrayTypes = definiteArrayTypes;
        }

        /// <summary>
        /// Gets the NamedTypes.
        /// </summary>
        /// <returns>The NamedTypes.</returns>
        public System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.NamedType> NamedTypes { get; }

        /// <summary>
        /// Gets the DefiniteArrayTypes.
        /// </summary>
        /// <returns>The DefiniteArrayTypes.</returns>
        public System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.DefiniteArrayType> DefiniteArrayTypes { get; }
    }
}