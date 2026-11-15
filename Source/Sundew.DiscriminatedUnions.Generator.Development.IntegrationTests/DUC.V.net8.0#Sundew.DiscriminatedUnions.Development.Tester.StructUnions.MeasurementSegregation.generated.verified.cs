//HintName: Sundew.DiscriminatedUnions.Development.Tester.StructUnions.MeasurementSegregation.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.StructUnions
{
    /// <summary>
    /// Contains a list for each case of <c>Measurement</c>, with the items that are that case.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
    public sealed partial class MeasurementSegregation
    {
        internal MeasurementSegregation(global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Unknown> unknowns, global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Quantity<int>> quantityOfInt32s, global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Quantity<double>> quantityOfDoubles)
        {
            this.Unknowns = unknowns;
            this.QuantityOfInt32s = quantityOfInt32s;
            this.QuantityOfDoubles = quantityOfDoubles;
        }

        /// <summary>
        /// Gets the items that are the <c>Unknown</c> case.
        /// </summary>
        public global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Unknown> Unknowns { get; }

        /// <summary>
        /// Gets the items that are the <c>Quantity&lt;int&gt;</c> case.
        /// </summary>
        public global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Quantity<int>> QuantityOfInt32s { get; }

        /// <summary>
        /// Gets the items that are the <c>Quantity&lt;double&gt;</c> case.
        /// </summary>
        public global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Quantity<double>> QuantityOfDoubles { get; }
    }
}
