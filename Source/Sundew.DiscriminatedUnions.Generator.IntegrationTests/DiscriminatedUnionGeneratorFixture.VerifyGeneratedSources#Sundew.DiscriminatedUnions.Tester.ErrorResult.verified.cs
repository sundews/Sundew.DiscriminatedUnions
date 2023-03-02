//HintName: Sundew.DiscriminatedUnions.Tester.ErrorResult.cs
namespace Sundew.DiscriminatedUnions.Tester
{
    partial record ErrorResult<T>
    {
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.Error<>))]
        public new static global::Sundew.DiscriminatedUnions.Tester.ErrorResult<T> Error(global::System.Int32 code) => new global::Sundew.DiscriminatedUnions.Tester.Error<T>(code);

        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.FatalError<>))]
        public new static global::Sundew.DiscriminatedUnions.Tester.ErrorResult<T> FatalError(global::System.Int32 code) => new global::Sundew.DiscriminatedUnions.Tester.FatalError<T>(code);
    }
}
