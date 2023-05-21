//HintName: Sundew.DiscriminatedUnions.Tester.ScopeForGenerator.cs
namespace Sundew.DiscriminatedUnions.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "4.0.0.0")]
    public partial record ScopeForGenerator
#pragma warning restore SA1601
    {
        /// <summary>
        /// Gets the Auto case.
        /// </summary>
        /// <returns>The Auto.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.ScopeForGenerator.Auto))]
        public static global::Sundew.DiscriminatedUnions.Tester.ScopeForGenerator AutoCase { get; } = new global::Sundew.DiscriminatedUnions.Tester.ScopeForGenerator.Auto();

        /// <summary>
        /// Factory method for the SingleInstancePerFuncResult case.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>A new SingleInstancePerFuncResult.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.ScopeForGenerator.SingleInstancePerFuncResult))]
        public static global::Sundew.DiscriminatedUnions.Tester.ScopeForGenerator SingleInstancePerFuncResultCase(string method) => new global::Sundew.DiscriminatedUnions.Tester.ScopeForGenerator.SingleInstancePerFuncResult(method);
    }
}
