//HintName: Sundew.DiscriminatedUnions.Tester.Input{TError}.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.3.0.0")]
    public partial record Input<TError>
#pragma warning restore SA1601
    {
        /// <summary>
        /// Factory method for the DoubleInput case.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A new DoubleInput.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.DoubleInput))]
        public static global::Sundew.DiscriminatedUnions.Tester.Input<global::Sundew.DiscriminatedUnions.Tester.DoubleError> DoubleInput(double value)
            => new global::Sundew.DiscriminatedUnions.Tester.DoubleInput(value);

        /// <summary>
        /// Factory method for the IntInput case.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A new IntInput.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.IntInput))]
        public static global::Sundew.DiscriminatedUnions.Tester.Input<global::Sundew.DiscriminatedUnions.Tester.IntError> IntInput(int value)
            => new global::Sundew.DiscriminatedUnions.Tester.IntInput(value);
    }
}
