//HintName: Sundew.DiscriminatedUnions.Tester.InterfaceUnion.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Tester
{
#pragma warning disable SA1601
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.3.0.0")]
    public partial interface InterfaceUnion
#pragma warning restore SA1601
    {
        /// <summary>
        /// Gets the One case.
        /// </summary>
        /// <returns>The One.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.InterfaceUnion.One))]
        [global::System.Diagnostics.DebuggerNonUserCode]
        public static global::Sundew.DiscriminatedUnions.Tester.InterfaceUnion _One { get; }
            = new global::Sundew.DiscriminatedUnions.Tester.InterfaceUnion.One();

        /// <summary>
        /// Gets the Two case.
        /// </summary>
        /// <returns>The Two.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.InterfaceUnion.Two))]
        [global::System.Diagnostics.DebuggerNonUserCode]
        public static global::Sundew.DiscriminatedUnions.Tester.InterfaceUnion _Two { get; }
            = new global::Sundew.DiscriminatedUnions.Tester.InterfaceUnion.Two();
    }
}
