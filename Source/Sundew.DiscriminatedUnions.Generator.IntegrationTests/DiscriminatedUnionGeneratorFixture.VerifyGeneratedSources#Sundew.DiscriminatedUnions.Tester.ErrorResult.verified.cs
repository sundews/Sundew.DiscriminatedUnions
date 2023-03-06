//HintName: Sundew.DiscriminatedUnions.Tester.ErrorResult.cs
namespace Sundew.DiscriminatedUnions.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "2.1.0.0")]
    public partial record ErrorResult<T>
#pragma warning restore SA1601
    {
        /// <summary>
        /// Factory method for the Error case.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>A new Error.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.Error<>))]
        public new static global::Sundew.DiscriminatedUnions.Tester.ErrorResult<T> Error(global::System.Int32 code) => new global::Sundew.DiscriminatedUnions.Tester.Error<T>(code);

        /// <summary>
        /// Factory method for the FatalError case.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>A new FatalError.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.FatalError<>))]
        public new static global::Sundew.DiscriminatedUnions.Tester.ErrorResult<T> FatalError(global::System.Int32 code) => new global::Sundew.DiscriminatedUnions.Tester.FatalError<T>(code);
    }
}
