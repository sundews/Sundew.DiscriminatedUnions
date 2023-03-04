//HintName: Sundew.DiscriminatedUnions.Tester.Result.cs
namespace Sundew.DiscriminatedUnions.Tester
{
    partial record Result<T>
    {
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.Success<>))]
        public static global::Sundew.DiscriminatedUnions.Tester.Result<T> Success(T value) => new global::Sundew.DiscriminatedUnions.Tester.Success<T>(value);

        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.Warning<>))]
        public static global::Sundew.DiscriminatedUnions.Tester.Result<T> Warning(global::System.String message) => new global::Sundew.DiscriminatedUnions.Tester.Warning<T>(message);

        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.Error<>))]
        public static global::Sundew.DiscriminatedUnions.Tester.Result<T> Error(global::System.Int32 code) => new global::Sundew.DiscriminatedUnions.Tester.Error<T>(code);

        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.FatalError<>))]
        public static global::Sundew.DiscriminatedUnions.Tester.Result<T> FatalError(global::System.Int32 code) => new global::Sundew.DiscriminatedUnions.Tester.FatalError<T>(code);
    }
}
