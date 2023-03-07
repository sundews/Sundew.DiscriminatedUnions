//HintName: Sundew.DiscriminatedUnions.Tester.ResultExtensions{T}.cs
namespace Sundew.DiscriminatedUnions.Tester
{
    /// <summary>
    /// Segregation extension method for Result.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "3.0.0.0")]
    public static partial class ResultExtensions
    {
        /// <summary>
        /// Segregates the items in the specified enumerable by type.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <returns>A new ResultSegregation.</returns>
        public static ResultSegregation<T> Segregate<T>(this System.Collections.Generic.IEnumerable<global::Sundew.DiscriminatedUnions.Tester.Result<T>> results)
        {
            var errors = new System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Tester.Error<T>>();
            var fatalErrors = new System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Tester.FatalError<T>>();
            var successes = new System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Tester.Success<T>>();
            var warnings = new System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Tester.Warning<T>>();

            foreach (var value in results)
            {
                switch (value)
                {
                    case global::Sundew.DiscriminatedUnions.Tester.Error<T> error:
                        errors.Add(error);
                        break;
                    case global::Sundew.DiscriminatedUnions.Tester.FatalError<T> fatalError:
                        fatalErrors.Add(fatalError);
                        break;
                    case global::Sundew.DiscriminatedUnions.Tester.Success<T> success:
                        successes.Add(success);
                        break;
                    case global::Sundew.DiscriminatedUnions.Tester.Warning<T> warning:
                        warnings.Add(warning);
                        break;
                }
            }

            return new global::Sundew.DiscriminatedUnions.Tester.ResultSegregation<T>(errors, fatalErrors, successes, warnings);
        }
    }
}