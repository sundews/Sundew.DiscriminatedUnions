//HintName: Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.BatchSegregation{TItem}.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions
{
    /// <summary>
    /// Contains a list for each case of <c>Batch&lt;TItem&gt;</c>, with the items that are that case.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
    public sealed partial class BatchSegregation<TItem>
    {
        internal BatchSegregation(global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.Completed> completed, global::System.Collections.Generic.IReadOnlyList<global::System.Collections.Generic.List<TItem>> lists)
        {
            this.Completed = completed;
            this.Lists = lists;
        }

        /// <summary>
        /// Gets the items that are the <c>Completed</c> case.
        /// </summary>
        public global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.Completed> Completed { get; }

        /// <summary>
        /// Gets the items that are the <c>List&lt;TItem&gt;</c> case.
        /// </summary>
        public global::System.Collections.Generic.IReadOnlyList<global::System.Collections.Generic.List<TItem>> Lists { get; }
    }
}
