//HintName: Sundew.DiscriminatedUnions.Development.Tester.StructUnions.CardinalityExtensions{TItem}.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.StructUnions
{
    /// <summary>
    /// Contains the extension method that segregates <c>Cardinality&lt;TItem&gt;</c>.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
    public static partial class CardinalityExtensions
    {
        /// <summary>
        /// Segregates the items in the specified enumerable by case.
        /// </summary>
        /// <param name="cardinalitys">The cardinalitys.</param>
        /// <returns>A new <c>CardinalitySegregation&lt;TItem&gt;</c>, which has a list for each case.</returns>
        public static global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.CardinalitySegregation<TItem> Segregate<TItem>(this global::System.Collections.Generic.IEnumerable<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Cardinality<TItem>> cardinalitys)
        {
            var emptys = new global::System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Empty<TItem>>();
            var singles = new global::System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Single<TItem>>();
            var multiples = new global::System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Multiple<TItem>>();

            foreach (var value in cardinalitys)
            {
                if (value.TryGetValue(out global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Empty<TItem> empty))
                {
                    emptys.Add(empty);
                }
                else if (value.TryGetValue(out global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Single<TItem> single))
                {
                    singles.Add(single);
                }
                else if (value.TryGetValue(out global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Multiple<TItem> multiple))
                {
                    multiples.Add(multiple);
                }
            }

            return new global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.CardinalitySegregation<TItem>(emptys, singles, multiples);
        }
    }
}
