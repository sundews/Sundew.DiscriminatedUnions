//HintName: Sundew.DiscriminatedUnions.Development.Tester.StructUnions.OutcomeExtensions.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.StructUnions
{
    /// <summary>
    /// Contains the extension method that segregates <c>Outcome</c>.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
    public static partial class OutcomeExtensions
    {
        /// <summary>
        /// Segregates the items in the specified enumerable by case.
        /// </summary>
        /// <param name="outcomes">The outcomes.</param>
        /// <returns>A new <c>OutcomeSegregation</c>, which has a list for each case.</returns>
        public static global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.OutcomeSegregation Segregate(this global::System.Collections.Generic.IEnumerable<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Outcome> outcomes)
        {
            var successes = new global::System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Success>();
            var strings = new global::System.Collections.Generic.List<string>();
            var exceptions = new global::System.Collections.Generic.List<global::System.Exception>();

            foreach (var value in outcomes)
            {
                if (value.TryGetValue(out global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Success success))
                {
                    successes.Add(success);
                }
                else if (value.TryGetValue(out string @string))
                {
                    strings.Add(@string);
                }
                else if (value.TryGetValue(out global::System.Exception exception))
                {
                    exceptions.Add(exception);
                }
            }

            return new global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.OutcomeSegregation(successes, strings, exceptions);
        }
    }
}
