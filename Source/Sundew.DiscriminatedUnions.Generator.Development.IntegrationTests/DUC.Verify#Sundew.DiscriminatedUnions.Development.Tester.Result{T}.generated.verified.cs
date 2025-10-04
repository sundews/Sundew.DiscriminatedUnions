//HintName: Sundew.DiscriminatedUnions.Development.Tester.Result{T}.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.3.0.0")]
    public partial record Result<T>
#pragma warning restore SA1601
    {
        /// <summary>
        /// Factory method for the Error case.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>A new Error.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.Error<>))]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.Result<T> Error(int code)
            => new global::Sundew.DiscriminatedUnions.Development.Tester.Error<T>(code);

        /// <summary>
        /// Factory method for the FatalError case.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>A new FatalError.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.FatalError<>))]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.Result<T> FatalError(int code)
            => new global::Sundew.DiscriminatedUnions.Development.Tester.FatalError<T>(code);

        /// <summary>
        /// Factory method for the Success case.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="optional">The optional.</param>
        /// <returns>A new Success.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.Success<>))]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.Result<T> Success(T value, T? optional)
            => new global::Sundew.DiscriminatedUnions.Development.Tester.Success<T>(value, optional);

        /// <summary>
        /// Factory method for the Warning case.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A new Warning.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.Warning<>))]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.Result<T> Warning(string message)
            => new global::Sundew.DiscriminatedUnions.Development.Tester.Warning<T>(message);
    }
}
