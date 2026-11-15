//HintName: Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.CompletionSegregation.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions
{
    /// <summary>
    /// Contains a list for each case of <c>Completion</c>, with the items that are that case.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
    public sealed partial class CompletionSegregation
    {
        internal CompletionSegregation(global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.Completed> completed, global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.Failed> failed)
        {
            this.Completed = completed;
            this.Failed = failed;
        }

        /// <summary>
        /// Gets the items that are the <c>Completed</c> case.
        /// </summary>
        public global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.Completed> Completed { get; }

        /// <summary>
        /// Gets the items that are the <c>Failed</c> case.
        /// </summary>
        public global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.Failed> Failed { get; }
    }
}
