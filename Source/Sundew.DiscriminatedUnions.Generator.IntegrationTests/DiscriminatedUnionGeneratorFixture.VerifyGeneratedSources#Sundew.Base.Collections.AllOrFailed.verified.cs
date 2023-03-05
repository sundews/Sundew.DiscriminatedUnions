//HintName: Sundew.Base.Collections.AllOrFailed.cs
namespace Sundew.Base.Collections
{
    partial class AllOrFailed<TItem, TResult, TError>
    {
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.Base.Collections.All<,,>))]
        public static global::Sundew.Base.Collections.AllOrFailed<TItem, TResult, TError> All(TResult[] items) => new global::Sundew.Base.Collections.All<TItem, TResult, TError>(items);

        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.Base.Collections.Failed<,,>))]
        public static global::Sundew.Base.Collections.AllOrFailed<TItem, TResult, TError> Failed(global::System.Collections.Generic.IReadOnlyList<global::Sundew.Base.Collections.FailedItem<TItem, TError>> items) => new global::Sundew.Base.Collections.Failed<TItem, TResult, TError>(items);
    }
}
