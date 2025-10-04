//HintName: Sundew.DiscriminatedUnions.Development.Tester.AllOrFailedExtensions{TItem,TResult,TError}.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester
{
    /// <summary>
    /// Segregation extension method for AllOrFailed.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.3.0.0")]
    public static partial class AllOrFailedExtensions
    {
        /// <summary>
        /// Segregates the items in the specified enumerable by type.
        /// </summary>
        /// <param name="allOrFailed">The allOrFailed.</param>
        /// <returns>A new AllOrFailedSegregation.</returns>
        public static AllOrFailedSegregation<TItem, TResult, TError> Segregate<TItem, TResult, TError>(this System.Collections.Generic.IEnumerable<global::Sundew.DiscriminatedUnions.Development.Tester.AllOrFailed<TItem, TResult, TError>> allOrFailed)
            where TItem : class, global::System.IEquatable<TItem>
            where TResult : struct
            where TError : global::System.Exception, TItem
        {
            var alls = new System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Development.Tester.All<TItem, TResult, TError>>();
            var faileds = new System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Development.Tester.Failed<TItem, TResult, TError>>();

            foreach (var value in allOrFailed)
            {
                switch (value)
                {
                    case global::Sundew.DiscriminatedUnions.Development.Tester.All<TItem, TResult, TError> all:
                        alls.Add(all);
                        break;
                    case global::Sundew.DiscriminatedUnions.Development.Tester.Failed<TItem, TResult, TError> failed:
                        faileds.Add(failed);
                        break;
                }
            }

            return new global::Sundew.DiscriminatedUnions.Development.Tester.AllOrFailedSegregation<TItem, TResult, TError>(alls, faileds);
        }
    }
}