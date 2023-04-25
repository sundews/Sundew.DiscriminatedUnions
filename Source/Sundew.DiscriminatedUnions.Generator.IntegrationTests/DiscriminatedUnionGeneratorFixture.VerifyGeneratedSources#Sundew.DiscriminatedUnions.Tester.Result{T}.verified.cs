//HintName: Sundew.DiscriminatedUnions.Tester.Result{T}.cs
namespace Sundew.DiscriminatedUnions.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "3.1.0.0")]
    public partial record Result<T>
#pragma warning restore SA1601
    {
        /// <summary>
        /// Factory method for the Error case.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>A new Error.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.Error<>))]
        public static global::Sundew.DiscriminatedUnions.Tester.Result<T> Error(int code) => new global::Sundew.DiscriminatedUnions.Tester.Error<T>(code);

        /// <summary>
        /// Factory method for the FatalError case.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>A new FatalError.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.FatalError<>))]
        public static global::Sundew.DiscriminatedUnions.Tester.Result<T> FatalError(int code) => new global::Sundew.DiscriminatedUnions.Tester.FatalError<T>(code);

        /// <summary>
        /// Factory method for the Success case.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="optional">The optional.</param>
        /// <returns>A new Success.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.Success<>))]
        public static global::Sundew.DiscriminatedUnions.Tester.Result<T> Success(T value, T? optional) => new global::Sundew.DiscriminatedUnions.Tester.Success<T>(value, optional);

        /// <summary>
        /// Factory method for the Warning case.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A new Warning.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.Warning<>))]
        public static global::Sundew.DiscriminatedUnions.Tester.Result<T> Warning(string message) => new global::Sundew.DiscriminatedUnions.Tester.Warning<T>(message);
    }
}
