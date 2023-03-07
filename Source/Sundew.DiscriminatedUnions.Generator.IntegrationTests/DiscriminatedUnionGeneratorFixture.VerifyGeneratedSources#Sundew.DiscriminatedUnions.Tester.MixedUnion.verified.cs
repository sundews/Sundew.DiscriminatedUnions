//HintName: Sundew.DiscriminatedUnions.Tester.MixedUnion.cs
namespace Sundew.DiscriminatedUnions.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "3.0.0.0")]
    public partial record MixedUnion
#pragma warning restore SA1601
    {
        /// <summary>
        /// Factory method for the Case2 case.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <returns>A new Case2.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.Case2))]
        public static global::Sundew.DiscriminatedUnions.Tester.MixedUnion Case2(string info) => new global::Sundew.DiscriminatedUnions.Tester.Case2(info);
    }
}
