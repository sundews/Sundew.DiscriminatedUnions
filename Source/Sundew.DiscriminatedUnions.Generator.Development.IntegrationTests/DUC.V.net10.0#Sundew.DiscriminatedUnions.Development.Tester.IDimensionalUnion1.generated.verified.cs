//HintName: Sundew.DiscriminatedUnions.Development.Tester.IDimensionalUnion1.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester
{
#pragma warning disable SA1601
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "6.0.0.0")]
    public partial interface IDimensionalUnion1 : global::Sundew.DiscriminatedUnions.IDiscriminatedUnion
#pragma warning restore SA1601
    {
        /// <summary>
        /// Factory method for the TwoDimensionalUnion case.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>A new TwoDimensionalUnion.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.TwoDimensionalUnion))]
        [global::System.Diagnostics.DebuggerNonUserCode]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.IDimensionalUnion1 TwoDimensionalUnion(int number)
            => new global::Sundew.DiscriminatedUnions.Development.Tester.TwoDimensionalUnion(number);

        /// <summary>
        /// Gets all cases in the union.
        /// </summary>
        /// <returns>A readonly list of types.</returns>
        public new static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
            = new global::System.Type[] { typeof(global::Sundew.DiscriminatedUnions.Development.Tester.TwoDimensionalUnion) };

        /// <summary>
        /// Gets all cases in the union.
        /// </summary>
        /// <returns>A readonly list of types.</returns>
        static global::System.Collections.Generic.IReadOnlyList<global::System.Type> global::Sundew.DiscriminatedUnions.IDiscriminatedUnion.Cases => Cases;
    }
}
