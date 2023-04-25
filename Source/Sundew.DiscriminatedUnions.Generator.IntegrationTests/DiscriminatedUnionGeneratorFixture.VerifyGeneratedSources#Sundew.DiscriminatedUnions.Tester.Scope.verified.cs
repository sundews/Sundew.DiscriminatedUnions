//HintName: Sundew.DiscriminatedUnions.Tester.Scope.cs
namespace Sundew.DiscriminatedUnions.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "3.1.0.0")]
    public partial record Scope
#pragma warning restore SA1601
    {
        /// <summary>
        /// Factory method for the Auto case.
        /// </summary>
        /// <returns>A new Auto.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.AutoScope))]
        public static global::Sundew.DiscriminatedUnions.Tester.Scope AutoScope { get; } = new global::Sundew.DiscriminatedUnions.Tester.AutoScope();

        /// <summary>
        /// Factory method for the SingleInstancePerFuncResult case.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>A new SingleInstancePerFuncResult.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.SingleInstancePerFuncResultScope))]
        public static global::Sundew.DiscriminatedUnions.Tester.Scope SingleInstancePerFuncResultScope(string method) => new global::Sundew.DiscriminatedUnions.Tester.SingleInstancePerFuncResultScope(method);
    }
}
