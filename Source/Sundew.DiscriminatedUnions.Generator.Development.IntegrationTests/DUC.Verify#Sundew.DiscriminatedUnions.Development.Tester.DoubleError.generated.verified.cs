//HintName: Sundew.DiscriminatedUnions.Development.Tester.DoubleError.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.3.0.0")]
    public partial record DoubleError
#pragma warning restore SA1601
    {
        /// <summary>
        /// Gets the OutOfRangeError case.
        /// </summary>
        /// <returns>The OutOfRangeError.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.DoubleError.OutOfRangeError))]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.DoubleError _OutOfRangeError { get; }
            = new global::Sundew.DiscriminatedUnions.Development.Tester.DoubleError.OutOfRangeError();

        /// <summary>
        /// Gets the RoundingError case.
        /// </summary>
        /// <returns>The RoundingError.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.DoubleError.RoundingError))]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.DoubleError _RoundingError { get; }
            = new global::Sundew.DiscriminatedUnions.Development.Tester.DoubleError.RoundingError();
    }
}
