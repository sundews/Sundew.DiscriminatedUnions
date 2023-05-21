//HintName: Sundew.Base.Collections.AllOrFailedSegregation{TItem,TResult,TError}.cs
namespace Sundew.Base.Collections
{
    /// <summary>
    /// Contains individual lists of the different cases of the discriminated union AllOrFailed.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "4.0.0.0")]
    public sealed partial class AllOrFailedSegregation<TItem, TResult, TError>
        where TItem : class, global::System.IEquatable<TItem>
        where TResult : struct
        where TError : global::System.Exception, TItem
    {
        internal AllOrFailedSegregation(System.Collections.Generic.IReadOnlyList<global::Sundew.Base.Collections.All<TItem, TResult, TError>> all, System.Collections.Generic.IReadOnlyList<global::Sundew.Base.Collections.Failed<TItem, TResult, TError>> failed)
        {
            this.All = all;
            this.Failed = failed;
        }

        /// <summary>
        /// Gets the All.
        /// </summary>
        /// <returns>The All.</returns>
        public System.Collections.Generic.IReadOnlyList<global::Sundew.Base.Collections.All<TItem, TResult, TError>> All { get; }

        /// <summary>
        /// Gets the Failed.
        /// </summary>
        /// <returns>The Failed.</returns>
        public System.Collections.Generic.IReadOnlyList<global::Sundew.Base.Collections.Failed<TItem, TResult, TError>> Failed { get; }
    }
}