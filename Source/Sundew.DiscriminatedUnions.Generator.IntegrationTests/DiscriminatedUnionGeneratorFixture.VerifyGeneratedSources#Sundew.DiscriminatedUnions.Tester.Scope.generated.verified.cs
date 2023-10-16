//HintName: Sundew.DiscriminatedUnions.Tester.Scope.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.1.0.0")]
    public partial record Scope
#pragma warning restore SA1601
    {
        /// <summary>
        /// Gets the AutoScope case.
        /// </summary>
        /// <returns>The AutoScope.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.Scope.AutoScope))]
        public static global::Sundew.DiscriminatedUnions.Tester.Scope _AutoScope { get; }
            = new global::Sundew.DiscriminatedUnions.Tester.Scope.AutoScope();

        /// <summary>
        /// Factory method for the SingleInstancePerFuncResultScope case.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>A new SingleInstancePerFuncResultScope.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.Scope.SingleInstancePerFuncResultScope))]
        public static global::Sundew.DiscriminatedUnions.Tester.Scope _SingleInstancePerFuncResultScope(string method)
            => new global::Sundew.DiscriminatedUnions.Tester.Scope.SingleInstancePerFuncResultScope(method);
    }
}
