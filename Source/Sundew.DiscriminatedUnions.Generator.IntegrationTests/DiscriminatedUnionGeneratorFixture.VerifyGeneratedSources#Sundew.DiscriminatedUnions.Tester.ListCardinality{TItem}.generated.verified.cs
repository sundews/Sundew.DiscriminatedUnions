﻿//HintName: Sundew.DiscriminatedUnions.Tester.ListCardinality{TItem}.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Tester
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
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.Empty<>))]
        public static global::Sundew.DiscriminatedUnions.Tester.ListCardinality<TItem> Empty { get; }
            = new global::Sundew.DiscriminatedUnions.Tester.Empty<TItem>();

        /// <summary>
        /// Factory method for the Multiple case.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>A new Multiple.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.Multiple<>))]
        public static global::Sundew.DiscriminatedUnions.Tester.ListCardinality<TItem> Multiple(global::System.Collections.Generic.IEnumerable<TItem> items)
            => new global::Sundew.DiscriminatedUnions.Tester.Multiple<TItem>(items);

        /// <summary>
        /// Factory method for the Single case.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>A new Single.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.Single<>))]
        public static global::Sundew.DiscriminatedUnions.Tester.ListCardinality<TItem> Single(TItem item)
            => new global::Sundew.DiscriminatedUnions.Tester.Single<TItem>(item);
    }
}
