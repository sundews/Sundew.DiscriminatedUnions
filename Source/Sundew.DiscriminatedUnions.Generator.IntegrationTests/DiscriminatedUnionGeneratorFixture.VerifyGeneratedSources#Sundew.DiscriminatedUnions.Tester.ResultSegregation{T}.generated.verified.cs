//HintName: Sundew.DiscriminatedUnions.Tester.ResultSegregation{T}.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Tester
{
    /// <summary>
    /// Contains individual lists of the different cases of the discriminated union Result.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.2.0.0")]
    public sealed partial class ResultSegregation<T>
    {
        internal ResultSegregation(System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.Error<T>> errors, System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.FatalError<T>> fatalErrors, System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.Success<T>> successes, System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.Warning<T>> warnings)
        {
            this.Errors = errors;
            this.FatalErrors = fatalErrors;
            this.Successes = successes;
            this.Warnings = warnings;
        }

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
    }
}