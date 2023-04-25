//HintName: Sundew.DiscriminatedUnions.Tester.MixedUnion.cs
namespace Sundew.DiscriminatedUnions.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "3.1.0.0")]
    public partial record MixedUnion
#pragma warning restore SA1601
    {
        /// <summary>
        /// Factory method for the Case2 case.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="optionalList">The optionalList.</param>
        /// <param name="optionalInts">The optionalInts.</param>
        /// <returns>A new Case2.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.Case2))]
        public static global::Sundew.DiscriminatedUnions.Tester.MixedUnion Case2(int? info = 4, global::System.Collections.Generic.List<string?>? optionalList = null, global::System.Collections.Generic.List<int?>? optionalInts = null) => new global::Sundew.DiscriminatedUnions.Tester.Case2(info, optionalList, optionalInts);

        /// <summary>
        /// Gets the Case3 case.
        /// </summary>
        /// <returns>The Case3.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.Case3))]
        public static global::Sundew.DiscriminatedUnions.Tester.MixedUnion Case3 { get; } = new global::Sundew.DiscriminatedUnions.Tester.Case3();
    }
}
