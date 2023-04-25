//HintName: Sundew.DiscriminatedUnions.Tester.ScopeForGenerator.cs
namespace Sundew.DiscriminatedUnions.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "3.1.0.0")]
    public partial record ScopeForGenerator
#pragma warning restore SA1601
    {
        /// <summary>
        /// Gets the AutoScopeForGenerator case.
        /// </summary>
        /// <returns>The AutoScopeForGenerator.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.ScopeForGenerator.AutoScopeForGenerator))]
        public static global::Sundew.DiscriminatedUnions.Tester.ScopeForGenerator Auto { get; } = new global::Sundew.DiscriminatedUnions.Tester.ScopeForGenerator.AutoScopeForGenerator();

        /// <summary>
        /// Factory method for the SingleInstancePerFuncResultScopeForGenerator case.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>A new SingleInstancePerFuncResultScopeForGenerator.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.ScopeForGenerator.SingleInstancePerFuncResultScopeForGenerator))]
        public static global::Sundew.DiscriminatedUnions.Tester.ScopeForGenerator SingleInstancePerFuncResult(string method) => new global::Sundew.DiscriminatedUnions.Tester.ScopeForGenerator.SingleInstancePerFuncResultScopeForGenerator(method);
    }
}
