//HintName: Sundew.DiscriminatedUnions.Development.Tester.Scope.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.4.0.0")]
    public partial record Scope : global::Sundew.DiscriminatedUnions.IDiscriminatedUnion
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

        /// <summary>
        /// Gets all cases in the union.
        /// </summary>
        /// <returns>A readonly list of types.</returns>
        public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
            = new global::System.Type[] { typeof(global::Sundew.DiscriminatedUnions.Development.Tester.Scope.AutoScope), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.Scope.SingleInstancePerFuncResultScope) };
    }
}
