//HintName: Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.SortedSegregation.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions
{
    /// <summary>
    /// Contains a list for each case of <c>Sorted</c>, with the items that are that case.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
    public sealed partial class SortedSegregation
    {
        internal SortedSegregation(global::System.Collections.Generic.IReadOnlyList<global::System.Exception> exceptions, global::System.Collections.Generic.IReadOnlyList<global::System.InvalidOperationException> invalidOperationExceptions, global::System.Collections.Generic.IReadOnlyList<int?> int32s)
        {
            this.Exceptions = exceptions;
            this.InvalidOperationExceptions = invalidOperationExceptions;
            this.Int32s = int32s;
        }

        /// <summary>
        /// Gets the items that are the <c>Exception</c> case.
        /// </summary>
        public global::System.Collections.Generic.IReadOnlyList<global::System.Exception> Exceptions { get; }

        /// <summary>
        /// Gets the items that are the <c>InvalidOperationException</c> case.
        /// </summary>
        public global::System.Collections.Generic.IReadOnlyList<global::System.InvalidOperationException> InvalidOperationExceptions { get; }

        /// <summary>
        /// Gets the items that are the <c>int?</c> case.
        /// </summary>
        public global::System.Collections.Generic.IReadOnlyList<int?> Int32s { get; }
    }
}
