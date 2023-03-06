//HintName: Sundew.Base.Collections.AllOrFailed.cs
namespace Sundew.Base.Collections
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "2.1.0.0")]
    public partial class AllOrFailed<TItem, TResult, TError>
        where TItem : class, global::System.IEquatable<TItem>
        where TResult : struct
        where TError : global::System.Exception, TItem
#pragma warning restore SA1601
    {
        /// <summary>
        /// Factory method for the All case.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="items">The items.</param>
        /// <returns>A new All.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.Base.Collections.All<,,>))]
        public static global::Sundew.Base.Collections.AllOrFailed<TItem, TResult, TError> All(TResult[] items) => new global::Sundew.Base.Collections.All<TItem, TResult, TError>(items);

        /// <summary>
        /// Factory method for the Failed case.
        /// </summary>
        /// <typeparam name="TItem">The type of the item.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="items">The items.</param>
        /// <returns>A new Failed.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.Base.Collections.Failed<,,>))]
        public static global::Sundew.Base.Collections.AllOrFailed<TItem, TResult, TError> Failed(global::System.Collections.Generic.IReadOnlyList<global::Sundew.Base.Collections.FailedItem<TItem, TError>> items) => new global::Sundew.Base.Collections.Failed<TItem, TResult, TError>(items);
    }
}
