//HintName: Sundew.DiscriminatedUnions.Development.Tester.DefiniteType.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "6.0.0.0")]
    public partial record DefiniteType
#pragma warning restore SA1601
    {
        /// <summary>
        /// Factory method for the DefiniteArrayType case.
        /// </summary>
        /// <param name="elementType">The elementType.</param>
        /// <returns>A new DefiniteArrayType.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.DefiniteArrayType))]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.DefiniteType DefiniteArrayType(global::Sundew.DiscriminatedUnions.Development.Tester.DefiniteType elementType)
            => new global::Sundew.DiscriminatedUnions.Development.Tester.DefiniteArrayType(elementType);

        /// <summary>
        /// Factory method for the NamedType case.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="namespace">The namespace.</param>
        /// <param name="assemblyName">The assemblyName.</param>
        /// <returns>A new NamedType.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.NamedType))]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.DefiniteType NamedType(string name, string @namespace, string? assemblyName)
            => new global::Sundew.DiscriminatedUnions.Development.Tester.NamedType(name, @namespace, assemblyName);

        /// <summary>
        /// Gets all cases in the union.
        /// </summary>
        /// <returns>A readonly list of types.</returns>
        public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
            = new global::System.Type[] { typeof(global::Sundew.DiscriminatedUnions.Development.Tester.DefiniteArrayType), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.NamedType) };
    }
}
