﻿//HintName: Sundew.DiscriminatedUnions.Development.Tester.ScopeForGenerator.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.3.0.0")]
    public partial record ScopeForGenerator
#pragma warning restore SA1601
    {
        /// <summary>
        /// Gets the Auto case.
        /// </summary>
        /// <returns>The Auto.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.ScopeForGenerator.Auto))]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.ScopeForGenerator _Auto { get; }
            = new global::Sundew.DiscriminatedUnions.Development.Tester.ScopeForGenerator.Auto();

        /// <summary>
        /// Factory method for the SingleInstancePerFuncResult case.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>A new SingleInstancePerFuncResult.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.ScopeForGenerator.SingleInstancePerFuncResult))]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.ScopeForGenerator _SingleInstancePerFuncResult(string method)
            => new global::Sundew.DiscriminatedUnions.Development.Tester.ScopeForGenerator.SingleInstancePerFuncResult(method);
    }
}
