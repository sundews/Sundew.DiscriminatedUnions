//HintName: Sundew.DiscriminatedUnions.Development.Tester.StructUnions.OutcomeSegregation.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.StructUnions
{
    /// <summary>
    /// Contains a list for each case of <c>Outcome</c>, with the items that are that case.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
    public sealed partial class OutcomeSegregation
    {
        internal OutcomeSegregation(global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Success> successes, global::System.Collections.Generic.IReadOnlyList<string> strings, global::System.Collections.Generic.IReadOnlyList<global::System.Exception> exceptions)
        {
            this.Successes = successes;
            this.Strings = strings;
            this.Exceptions = exceptions;
        }

        /// <summary>
        /// Gets the items that are the <c>Success</c> case.
        /// </summary>
        public global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Success> Successes { get; }

        /// <summary>
        /// Gets the items that are the <c>string</c> case.
        /// </summary>
        public global::System.Collections.Generic.IReadOnlyList<string> Strings { get; }

        /// <summary>
        /// Gets the items that are the <c>Exception</c> case.
        /// </summary>
        public global::System.Collections.Generic.IReadOnlyList<global::System.Exception> Exceptions { get; }
    }
}
