//HintName: Sundew.DiscriminatedUnions.Development.Tester.StructUnions.CardinalitySegregation{TItem}.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.StructUnions
{
    /// <summary>
    /// Contains a list for each case of <c>Cardinality&lt;TItem&gt;</c>, with the items that are that case.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
    public sealed partial class CardinalitySegregation<TItem>
    {
        internal CardinalitySegregation(global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Empty<TItem>> emptys, global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Single<TItem>> singles, global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Multiple<TItem>> multiples)
        {
            this.Emptys = emptys;
            this.Singles = singles;
            this.Multiples = multiples;
        }

        /// <summary>
        /// Gets the items that are the <c>Empty&lt;TItem&gt;</c> case.
        /// </summary>
        public global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Empty<TItem>> Emptys { get; }

        /// <summary>
        /// Gets the items that are the <c>Single&lt;TItem&gt;</c> case.
        /// </summary>
        public global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Single<TItem>> Singles { get; }

        /// <summary>
        /// Gets the items that are the <c>Multiple&lt;TItem&gt;</c> case.
        /// </summary>
        public global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Multiple<TItem>> Multiples { get; }
    }
}
