//HintName: Sundew.DiscriminatedUnions.Development.Tester.InterfaceUnion.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester
{
#pragma warning disable SA1601
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.4.0.0")]
    public partial interface InterfaceUnion
#pragma warning restore SA1601
    {
        /// <summary>
        /// Gets the One case.
        /// </summary>
        /// <returns>The One.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.InterfaceUnion.One))]
        [global::System.Diagnostics.DebuggerNonUserCode]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.InterfaceUnion _One { get; }
            = new global::Sundew.DiscriminatedUnions.Development.Tester.InterfaceUnion.One();

        /// <summary>
        /// Gets the Two case.
        /// </summary>
        /// <returns>The Two.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.InterfaceUnion.Two))]
        [global::System.Diagnostics.DebuggerNonUserCode]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.InterfaceUnion _Two { get; }
            = new global::Sundew.DiscriminatedUnions.Development.Tester.InterfaceUnion.Two();

        /// <summary>
        /// Gets all cases in the union.
        /// </summary>
        /// <returns>A readonly list of types.</returns>
        public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
            = new global::System.Type[] { typeof(global::Sundew.DiscriminatedUnions.Development.Tester.InterfaceUnion.One), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.InterfaceUnion.Two) };
    }
}
