//HintName: Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.BatchExtensions{TItem}.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions
{
    /// <summary>
    /// Contains the extension method that segregates <c>Batch&lt;TItem&gt;</c>.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
    public static partial class BatchExtensions
    {
        /// <summary>
        /// Segregates the items in the specified enumerable by case.
        /// </summary>
        /// <param name="batchs">The batchs.</param>
        /// <returns>A new <c>BatchSegregation&lt;TItem&gt;</c>, which has a list for each case.</returns>
        public static global::Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.BatchSegregation<TItem> Segregate<TItem>(this global::System.Collections.Generic.IEnumerable<global::Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.Batch<TItem>> batchs)
        {
            var completeds = new global::System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.Completed>();
            var lists = new global::System.Collections.Generic.List<global::System.Collections.Generic.List<TItem>>();

            foreach (var value in batchs)
            {
                switch (value.Value)
                {
                    case global::Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.Completed completed:
                        completeds.Add(completed);
                        break;
                    case global::System.Collections.Generic.List<TItem> list:
                        lists.Add(list);
                        break;
                }
            }

            return new global::Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.BatchSegregation<TItem>(completeds, lists);
        }
    }
}
