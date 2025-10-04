//HintName: Sundew.DiscriminatedUnions.Development.Tester.AllOrFailedSegregation{TItem,TResult,TError}.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester
{
    /// <summary>
    /// Contains individual lists of the different cases of the discriminated union AllOrFailed.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.3.0.0")]
    public sealed partial class AllOrFailedSegregation<TItem, TResult, TError>
        where TItem : class, global::System.IEquatable<TItem>
        where TResult : struct
        where TError : global::System.Exception, TItem
    {
        internal AllOrFailedSegregation(System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.All<TItem, TResult, TError>> all, System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.Failed<TItem, TResult, TError>> failed)
        {
            this.All = all;
            this.Failed = failed;
        }

        /// <summary>
        /// Gets the All.
        /// </summary>
        /// <returns>The All.</returns>
        public System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.All<TItem, TResult, TError>> All { get; }

        /// <summary>
        /// Gets the Failed.
        /// </summary>
        /// <returns>The Failed.</returns>
        public System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.Failed<TItem, TResult, TError>> Failed { get; }
    }
}