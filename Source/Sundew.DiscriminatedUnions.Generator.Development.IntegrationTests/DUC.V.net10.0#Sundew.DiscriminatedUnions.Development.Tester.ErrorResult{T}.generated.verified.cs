//HintName: Sundew.DiscriminatedUnions.Development.Tester.ErrorResult{T}.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.4.0.0")]
    public partial record ErrorResult<T> : global::Sundew.DiscriminatedUnions.IDiscriminatedUnion
#pragma warning restore SA1601
    {
        /// <summary>
        /// Factory method for the Error case.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>A new Error.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.Error<>))]
        public new static global::Sundew.DiscriminatedUnions.Development.Tester.ErrorResult<T> Error(int code)
            => new global::Sundew.DiscriminatedUnions.Development.Tester.Error<T>(code);

        /// <summary>
        /// Factory method for the FatalError case.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>A new FatalError.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.FatalError<>))]
        public new static global::Sundew.DiscriminatedUnions.Development.Tester.ErrorResult<T> FatalError(int code)
            => new global::Sundew.DiscriminatedUnions.Development.Tester.FatalError<T>(code);

        /// <summary>
        /// Gets all cases in the union.
        /// </summary>
        /// <returns>A readonly list of types.</returns>
        public new static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
            = new global::System.Type[] { typeof(global::Sundew.DiscriminatedUnions.Development.Tester.Error<T>), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.FatalError<T>) };
    }
}
