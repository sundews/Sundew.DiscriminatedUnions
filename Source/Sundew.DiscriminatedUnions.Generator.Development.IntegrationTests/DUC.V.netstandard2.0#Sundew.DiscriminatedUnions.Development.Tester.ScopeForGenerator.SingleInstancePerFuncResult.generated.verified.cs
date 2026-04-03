//HintName: Sundew.DiscriminatedUnions.Development.Tester.ScopeForGenerator.SingleInstancePerFuncResult.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester
{
    public partial record ScopeForGenerator
    {
#pragma warning disable SA1601
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "6.0.0.0")]
        internal partial record SingleInstancePerFuncResult
#pragma warning restore SA1601
        {
            /// <summary>
            /// Gets all cases in the union.
            /// </summary>
            /// <returns>A readonly list of types.</returns>
            public new static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
                = new global::System.Type[] { typeof(global::Sundew.DiscriminatedUnions.Development.Tester.ScopeForGenerator.SingleInstancePerFuncResult) };
        }
    }
}
