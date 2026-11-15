//HintName: Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.AttemptSegregation{TValue,TError}.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes
{
    /// <summary>
    /// Contains a list for each case of <c>Attempt&lt;TValue, TError&gt;</c>, with the items that are that case.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
    public sealed partial class AttemptSegregation<TValue, TError>
        where TValue : notnull
    {
        internal AttemptSegregation(global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Attempt<TValue, TError>.Succeeded> succeeded, global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Faulted<TValue, TError>> faulted)
        {
            this.Succeeded = succeeded;
            this.Faulted = faulted;
        }

        /// <summary>
        /// Gets the items that are the <c>Attempt&lt;TValue, TError&gt;.Succeeded</c> case.
        /// </summary>
        public global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Attempt<TValue, TError>.Succeeded> Succeeded { get; }

        /// <summary>
        /// Gets the items that are the <c>Faulted&lt;TValue, TError&gt;</c> case.
        /// </summary>
        public global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Faulted<TValue, TError>> Faulted { get; }
    }
}
