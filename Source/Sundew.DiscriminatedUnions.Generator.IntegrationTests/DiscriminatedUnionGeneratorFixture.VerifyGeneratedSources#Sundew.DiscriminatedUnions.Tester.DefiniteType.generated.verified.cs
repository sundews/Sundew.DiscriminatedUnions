//HintName: Sundew.DiscriminatedUnions.Tester.DefiniteType.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.0.0.0")]
    public partial record DefiniteType
#pragma warning restore SA1601
    {
        /// <summary>
        /// Factory method for the DefiniteArrayType case.
        /// </summary>
        /// <param name="elementType">The elementType.</param>
        /// <returns>A new DefiniteArrayType.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.DefiniteArrayType))]
        public static global::Sundew.DiscriminatedUnions.Tester.DefiniteType DefiniteArrayType(global::Sundew.DiscriminatedUnions.Tester.DefiniteType elementType) => new global::Sundew.DiscriminatedUnions.Tester.DefiniteArrayType(elementType);

        /// <summary>
        /// Factory method for the NamedType case.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="namespace">The namespace.</param>
        /// <param name="assemblyName">The assemblyName.</param>
        /// <returns>A new NamedType.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.NamedType))]
        public static global::Sundew.DiscriminatedUnions.Tester.DefiniteType NamedType(string name, string @namespace, string? assemblyName) => new global::Sundew.DiscriminatedUnions.Tester.NamedType(name, @namespace, assemblyName);
    }
}
