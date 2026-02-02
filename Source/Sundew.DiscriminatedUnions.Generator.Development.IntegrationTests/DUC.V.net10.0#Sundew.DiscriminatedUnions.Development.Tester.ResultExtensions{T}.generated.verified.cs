//HintName: Sundew.DiscriminatedUnions.Development.Tester.ResultExtensions{T}.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester
{
    /// <summary>
    /// Segregation extension method for Result.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "6.0.0.0")]
    public static partial class ResultExtensions
    {
        /// <summary>
        /// Segregates the items in the specified enumerable by type.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <returns>A new ResultSegregation.</returns>
        public static ResultSegregation<T> Segregate<T>(this System.Collections.Generic.IEnumerable<global::Sundew.DiscriminatedUnions.Development.Tester.Result<T>> results)
        {
            var errors = new System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Development.Tester.Error<T>>();
            var fatalErrors = new System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Development.Tester.FatalError<T>>();
            var successes = new System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Development.Tester.Success<T>>();
            var warnings = new System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Development.Tester.Warning<T>>();

            foreach (var value in results)
            {
                switch (value)
                {
                    case global::Sundew.DiscriminatedUnions.Development.Tester.Error<T> error:
                        errors.Add(error);
                        break;
                    case global::Sundew.DiscriminatedUnions.Development.Tester.FatalError<T> fatalError:
                        fatalErrors.Add(fatalError);
                        break;
                    case global::Sundew.DiscriminatedUnions.Development.Tester.Success<T> success:
                        successes.Add(success);
                        break;
                    case global::Sundew.DiscriminatedUnions.Development.Tester.Warning<T> warning:
                        warnings.Add(warning);
                        break;
                }
            }

            return new global::Sundew.DiscriminatedUnions.Development.Tester.ResultSegregation<T>(errors, fatalErrors, successes, warnings);
        }
    }
}