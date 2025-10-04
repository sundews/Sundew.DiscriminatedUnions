//HintName: Sundew.DiscriminatedUnions.Development.Tester.ListCardinality{TItem}.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.3.0.0")]
    public partial class ListCardinality<TItem>
#pragma warning restore SA1601
    {
        /// <summary>
        /// Gets the Empty case.
        /// </summary>
        /// <returns>The Empty.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.Empty<>))]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.ListCardinality<TItem> Empty { get; }
            = new global::Sundew.DiscriminatedUnions.Development.Tester.Empty<TItem>();

        /// <summary>
        /// Factory method for the Multiple case.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>A new Multiple.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.Multiple<>))]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.ListCardinality<TItem> Multiple(global::System.Collections.Generic.IEnumerable<TItem> items)
            => new global::Sundew.DiscriminatedUnions.Development.Tester.Multiple<TItem>(items);

        /// <summary>
        /// Factory method for the Single case.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>A new Single.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.Single<>))]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.ListCardinality<TItem> Single(TItem item)
            => new global::Sundew.DiscriminatedUnions.Development.Tester.Single<TItem>(item);
    }
}
