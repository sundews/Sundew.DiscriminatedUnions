//HintName: Sundew.DiscriminatedUnions.Tester.ResultSegregation.cs
namespace Sundew.DiscriminatedUnions.Tester
{
    /// <summary>
    /// Contains individual lists of the different cases of the discriminated union Result.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "2.1.0.0")]
    public sealed class ResultSegregation<T>
    {
        internal ResultSegregation(System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.Success<T>> successes, System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.Warning<T>> warnings, System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.Error<T>> errors, System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.FatalError<T>> fatalErrors)
        {
            this.Successes = successes;
            this.Warnings = warnings;
            this.Errors = errors;
            this.FatalErrors = fatalErrors;
        }

        /// <summary>
        /// Gets the Successes.
        /// </summary>
        /// <returns>The Successes.</returns>
        public System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.Success<T>> Successes { get; }

        /// <summary>
        /// Gets the Warnings.
        /// </summary>
        /// <returns>The Warnings.</returns>
        public System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.Warning<T>> Warnings { get; }

        /// <summary>
        /// Gets the Errors.
        /// </summary>
        /// <returns>The Errors.</returns>
        public System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.Error<T>> Errors { get; }

        /// <summary>
        /// Gets the FatalErrors.
        /// </summary>
        /// <returns>The FatalErrors.</returns>
        public System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.FatalError<T>> FatalErrors { get; }
    }
}