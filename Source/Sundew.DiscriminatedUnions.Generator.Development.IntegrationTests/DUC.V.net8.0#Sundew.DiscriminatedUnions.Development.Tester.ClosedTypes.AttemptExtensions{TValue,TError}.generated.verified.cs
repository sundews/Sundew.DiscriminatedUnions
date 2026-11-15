//HintName: Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.AttemptExtensions{TValue,TError}.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes
{
    /// <summary>
    /// Contains the extension method that segregates <c>Attempt&lt;TValue, TError&gt;</c>.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
    public static partial class AttemptExtensions
    {
        /// <summary>
        /// Segregates the items in the specified enumerable by case.
        /// </summary>
        /// <param name="attempts">The attempts.</param>
        /// <returns>A new <c>AttemptSegregation&lt;TValue, TError&gt;</c>, which has a list for each case.</returns>
        public static global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.AttemptSegregation<TValue, TError> Segregate<TValue, TError>(this global::System.Collections.Generic.IEnumerable<global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Attempt<TValue, TError>> attempts)
            where TValue : notnull
        {
            var succeededs = new global::System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Attempt<TValue, TError>.Succeeded>();
            var faulteds = new global::System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Faulted<TValue, TError>>();

            foreach (var value in attempts)
            {
                switch (value)
                {
                    case global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Attempt<TValue, TError>.Succeeded succeeded:
                        succeededs.Add(succeeded);
                        break;
                    case global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Faulted<TValue, TError> faulted:
                        faulteds.Add(faulted);
                        break;
                }
            }

            return new global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.AttemptSegregation<TValue, TError>(succeededs, faulteds);
        }
    }
}
