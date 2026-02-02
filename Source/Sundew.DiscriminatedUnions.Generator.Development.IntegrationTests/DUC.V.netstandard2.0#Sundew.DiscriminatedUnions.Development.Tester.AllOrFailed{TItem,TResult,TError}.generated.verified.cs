//HintName: Sundew.DiscriminatedUnions.Development.Tester.AllOrFailed{TItem,TResult,TError}.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "6.0.0.0")]
    public partial class AllOrFailed<TItem, TResult, TError>
        where TItem : class, global::System.IEquatable<TItem>
        where TResult : struct
        where TError : global::System.Exception, TItem
#pragma warning restore SA1601
    {
        /// <summary>
        /// Factory method for the All case.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>A new All.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.All<,,>))]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.AllOrFailed<TItem, TResult, TError> All(TResult[] items)
            => new global::Sundew.DiscriminatedUnions.Development.Tester.All<TItem, TResult, TError>(items);

        /// <summary>
        /// Factory method for the Failed case.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>A new Failed.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.Failed<,,>))]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.AllOrFailed<TItem, TResult, TError> Failed(global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.FailedItem<TItem, TError>> items)
            => new global::Sundew.DiscriminatedUnions.Development.Tester.Failed<TItem, TResult, TError>(items);

        /// <summary>
        /// Gets all cases in the union.
        /// </summary>
        /// <returns>A readonly list of types.</returns>
        public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
            = new global::System.Type[] { typeof(global::Sundew.DiscriminatedUnions.Development.Tester.All<TItem, TResult, TError>), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.Failed<TItem, TResult, TError>) };
    }
}
