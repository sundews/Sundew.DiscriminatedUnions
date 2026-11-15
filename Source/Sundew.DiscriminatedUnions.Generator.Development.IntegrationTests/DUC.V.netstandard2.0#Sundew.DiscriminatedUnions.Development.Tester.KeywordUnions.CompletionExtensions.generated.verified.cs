//HintName: Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.CompletionExtensions.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions
{
    /// <summary>
    /// Contains the extension method that segregates <c>Completion</c>.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
    public static partial class CompletionExtensions
    {
        /// <summary>
        /// Segregates the items in the specified enumerable by case.
        /// </summary>
        /// <param name="completions">The completions.</param>
        /// <returns>A new <c>CompletionSegregation</c>, which has a list for each case.</returns>
        public static global::Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.CompletionSegregation Segregate(this global::System.Collections.Generic.IEnumerable<global::Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.Completion> completions)
        {
            var completeds = new global::System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.Completed>();
            var faileds = new global::System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.Failed>();

            foreach (var value in completions)
            {
                switch (value.Value)
                {
                    case global::Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.Completed completed:
                        completeds.Add(completed);
                        break;
                    case global::Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.Failed failed:
                        faileds.Add(failed);
                        break;
                }
            }

            return new global::Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.CompletionSegregation(completeds, faileds);
        }
    }
}
