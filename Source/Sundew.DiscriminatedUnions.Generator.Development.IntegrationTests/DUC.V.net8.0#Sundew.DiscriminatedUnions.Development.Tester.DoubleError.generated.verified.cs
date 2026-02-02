//HintName: Sundew.DiscriminatedUnions.Development.Tester.DoubleError.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "6.0.0.0")]
    public partial record DoubleError : global::Sundew.DiscriminatedUnions.IDiscriminatedUnion
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

        /// <summary>
        /// Gets all cases in the union.
        /// </summary>
        /// <returns>A readonly list of types.</returns>
        public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
            = new global::System.Type[] { typeof(global::Sundew.DiscriminatedUnions.Development.Tester.DoubleError.OutOfRangeError), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.DoubleError.RoundingError) };
    }
}
