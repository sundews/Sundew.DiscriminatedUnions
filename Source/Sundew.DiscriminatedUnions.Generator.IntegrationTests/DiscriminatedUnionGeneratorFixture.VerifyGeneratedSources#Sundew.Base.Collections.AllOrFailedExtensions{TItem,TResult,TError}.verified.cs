//HintName: Sundew.Base.Collections.AllOrFailedExtensions{TItem,TResult,TError}.cs
namespace Sundew.Base.Collections
{
    /// <summary>
    /// Segregation extension method for AllOrFailed.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "3.1.0.0")]
    public static partial class AllOrFailedExtensions
    {
        /// <summary>
        /// Segregates the items in the specified enumerable by type.
        /// </summary>
        /// <param name="allOrFailed">The allOrFailed.</param>
        /// <returns>A new AllOrFailedSegregation.</returns>
        public static AllOrFailedSegregation<TItem, TResult, TError> Segregate<TItem, TResult, TError>(this System.Collections.Generic.IEnumerable<global::Sundew.Base.Collections.AllOrFailed<TItem, TResult, TError>> allOrFailed)
            where TItem : class, global::System.IEquatable<TItem>
            where TResult : struct
            where TError : global::System.Exception, TItem
        {
            var alls = new System.Collections.Generic.List<global::Sundew.Base.Collections.All<TItem, TResult, TError>>();
            var faileds = new System.Collections.Generic.List<global::Sundew.Base.Collections.Failed<TItem, TResult, TError>>();

            foreach (var value in allOrFailed)
            {
                switch (value)
                {
                    case global::Sundew.Base.Collections.All<TItem, TResult, TError> all:
                        alls.Add(all);
                        break;
                    case global::Sundew.Base.Collections.Failed<TItem, TResult, TError> failed:
                        faileds.Add(failed);
                        break;
                }
            }

            return new global::Sundew.Base.Collections.AllOrFailedSegregation<TItem, TResult, TError>(alls, faileds);
        }
    }
}