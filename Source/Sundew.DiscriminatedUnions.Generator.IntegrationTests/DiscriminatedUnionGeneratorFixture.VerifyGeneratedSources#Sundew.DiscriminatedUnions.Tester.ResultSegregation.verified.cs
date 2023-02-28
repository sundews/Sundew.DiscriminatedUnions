//HintName: Sundew.DiscriminatedUnions.Tester.ResultSegregation.cs
namespace Sundew.DiscriminatedUnions.Tester
{
    public sealed class ResultSegregation
    {
        internal ResultSegregation(System.Collections.Generic.IReadOnlyList<Sundew.DiscriminatedUnions.Tester.Success> successes, System.Collections.Generic.IReadOnlyList<Sundew.DiscriminatedUnions.Tester.Warning> warnings, System.Collections.Generic.IReadOnlyList<Sundew.DiscriminatedUnions.Tester.Error> errors, System.Collections.Generic.IReadOnlyList<Sundew.DiscriminatedUnions.Tester.FatalError> fatalErrors)
        {
            this.Successes = successes;
            this.Warnings = warnings;
            this.Errors = errors;
            this.FatalErrors = fatalErrors;
        }

        public System.Collections.Generic.IReadOnlyList<Sundew.DiscriminatedUnions.Tester.Success> Successes { get; }

        public System.Collections.Generic.IReadOnlyList<Sundew.DiscriminatedUnions.Tester.Warning> Warnings { get; }

        public System.Collections.Generic.IReadOnlyList<Sundew.DiscriminatedUnions.Tester.Error> Errors { get; }

        public System.Collections.Generic.IReadOnlyList<Sundew.DiscriminatedUnions.Tester.FatalError> FatalErrors { get; }
    }
}