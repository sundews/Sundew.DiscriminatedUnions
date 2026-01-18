//HintName: Sundew.DiscriminatedUnions.Development.Tester.Input{TError}.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.4.0.0")]
    public partial record Input<TError>
#pragma warning restore SA1601
    {
        /// <summary>
        /// Factory method for the DoubleInput case.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A new DoubleInput.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.DoubleInput))]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.Input<global::Sundew.DiscriminatedUnions.Development.Tester.DoubleError> DoubleInput(double value)
            => new global::Sundew.DiscriminatedUnions.Development.Tester.DoubleInput(value);

        /// <summary>
        /// Factory method for the IntInput case.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A new IntInput.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.IntInput))]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.Input<global::Sundew.DiscriminatedUnions.Development.Tester.IntError> IntInput(int value)
            => new global::Sundew.DiscriminatedUnions.Development.Tester.IntInput(value);

        /// <summary>
        /// Gets all cases in the union.
        /// </summary>
        /// <returns>A readonly list of types.</returns>
        public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
            = new global::System.Type[] { typeof(global::Sundew.DiscriminatedUnions.Development.Tester.DoubleInput), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.IntInput) };
    }
}
