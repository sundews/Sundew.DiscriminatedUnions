//HintName: Sundew.DiscriminatedUnions.Tester.DefiniteTypeSegregation.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Tester
{
    /// <summary>
    /// Contains individual lists of the different cases of the discriminated union DefiniteType.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.3.0.0")]
    public sealed partial class DefiniteTypeSegregation
    {
        internal DefiniteTypeSegregation(System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.DefiniteArrayType> definiteArrayTypes, System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.NamedType> namedTypes)
        {
            this.DefiniteArrayTypes = definiteArrayTypes;
            this.NamedTypes = namedTypes;
        }

        /// <summary>
        /// Gets the DefiniteArrayTypes.
        /// </summary>
        /// <returns>The DefiniteArrayTypes.</returns>
        public System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.DefiniteArrayType> DefiniteArrayTypes { get; }

        /// <summary>
        /// Gets the NamedTypes.
        /// </summary>
        /// <returns>The NamedTypes.</returns>
        public System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.NamedType> NamedTypes { get; }
    }
}