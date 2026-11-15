//HintName: Sundew.DiscriminatedUnions.Development.Tester.StructUnions.MeasurementExtensions.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.StructUnions
{
    /// <summary>
    /// Contains the extension method that segregates <c>Measurement</c>.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
    public static partial class MeasurementExtensions
    {
        /// <summary>
        /// Segregates the items in the specified enumerable by case.
        /// </summary>
        /// <param name="measurements">The measurements.</param>
        /// <returns>A new <c>MeasurementSegregation</c>, which has a list for each case.</returns>
        public static global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.MeasurementSegregation Segregate(this global::System.Collections.Generic.IEnumerable<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Measurement> measurements)
        {
            var unknowns = new global::System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Unknown>();
            var quantityOfInt32s = new global::System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Quantity<int>>();
            var quantityOfDoubles = new global::System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Quantity<double>>();

            foreach (var value in measurements)
            {
                if (value.TryGetValue(out global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Unknown unknown))
                {
                    unknowns.Add(unknown);
                }
                else if (value.TryGetValue(out global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Quantity<int> quantityOfInt32))
                {
                    quantityOfInt32s.Add(quantityOfInt32);
                }
                else if (value.TryGetValue(out global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Quantity<double> quantityOfDouble))
                {
                    quantityOfDoubles.Add(quantityOfDouble);
                }
            }

            return new global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.MeasurementSegregation(unknowns, quantityOfInt32s, quantityOfDoubles);
        }
    }
}
