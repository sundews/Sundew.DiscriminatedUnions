//HintName: Sundew.DiscriminatedUnions.Tester.ResultSegregation.cs
namespace Sundew.DiscriminatedUnions.Tester
{
    public sealed class ResultSegregation<T>
    {
        internal ResultSegregation(System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.Success<T>> successes, System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.Warning<T>> warnings, System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.Error<T>> errors, System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.FatalError<T>> fatalErrors)
        {
            this.Successes = successes;
            this.Warnings = warnings;
            this.Errors = errors;
            this.FatalErrors = fatalErrors;
        }

        public System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.Success<T>> Successes { get; }

        public System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.Warning<T>> Warnings { get; }

        public System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.Error<T>> Errors { get; }

        public System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.FatalError<T>> FatalErrors { get; }
    }
}