//HintName: Sundew.DiscriminatedUnions.Tester.Result.cs
namespace Sundew.DiscriminatedUnions.Tester
{
#pragma warning disabled SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "2.1.0.0")]
    public partial record Result<T>
#pragma warning restore SA1601
    {
        /// <summary>
        /// Factory method for the Success case.
        /// </summary>
        /// <typeparam name="T">The type of the t.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>A new Success.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.Success<>))]
        public static global::Sundew.DiscriminatedUnions.Tester.Result<T> Success(T value) => new global::Sundew.DiscriminatedUnions.Tester.Success<T>(value);

        /// <summary>
        /// Factory method for the Warning case.
        /// </summary>
        /// <typeparam name="T">The type of the t.</typeparam>
        /// <param name="message">The message.</param>
        /// <returns>A new Warning.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.Warning<>))]
        public static global::Sundew.DiscriminatedUnions.Tester.Result<T> Warning(global::System.String message) => new global::Sundew.DiscriminatedUnions.Tester.Warning<T>(message);

        /// <summary>
        /// Factory method for the Error case.
        /// </summary>
        /// <typeparam name="T">The type of the t.</typeparam>
        /// <param name="code">The code.</param>
        /// <returns>A new Error.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.Error<>))]
        public static global::Sundew.DiscriminatedUnions.Tester.Result<T> Error(global::System.Int32 code) => new global::Sundew.DiscriminatedUnions.Tester.Error<T>(code);

        /// <summary>
        /// Factory method for the FatalError case.
        /// </summary>
        /// <typeparam name="T">The type of the t.</typeparam>
        /// <param name="code">The code.</param>
        /// <returns>A new FatalError.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.FatalError<>))]
        public static global::Sundew.DiscriminatedUnions.Tester.Result<T> FatalError(global::System.Int32 code) => new global::Sundew.DiscriminatedUnions.Tester.FatalError<T>(code);
    }
}
