//HintName: Sundew.DiscriminatedUnions.Development.Tester.Scope.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.3.0.0")]
    public partial record Scope
#pragma warning restore SA1601
    {
        /// <summary>
        /// Gets the AutoScope case.
        /// </summary>
        /// <returns>The AutoScope.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.Scope.AutoScope))]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.Scope _AutoScope { get; }
            = new global::Sundew.DiscriminatedUnions.Development.Tester.Scope.AutoScope();

        /// <summary>
        /// Factory method for the SingleInstancePerFuncResultScope case.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>A new SingleInstancePerFuncResultScope.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.Scope.SingleInstancePerFuncResultScope))]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.Scope _SingleInstancePerFuncResultScope(string method)
            => new global::Sundew.DiscriminatedUnions.Development.Tester.Scope.SingleInstancePerFuncResultScope(method);
    }
}
