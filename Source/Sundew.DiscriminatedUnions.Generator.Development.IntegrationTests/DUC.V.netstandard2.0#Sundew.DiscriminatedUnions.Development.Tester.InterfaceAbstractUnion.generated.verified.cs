//HintName: Sundew.DiscriminatedUnions.Development.Tester.InterfaceAbstractUnion.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.4.0.0")]
    public partial record InterfaceAbstractUnion
#pragma warning restore SA1601
    {
        /// <summary>
        /// Factory method for the RecordUnion case.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>A new RecordUnion.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.RecordUnion))]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.InterfaceAbstractUnion RecordUnion(int number)
            => new global::Sundew.DiscriminatedUnions.Development.Tester.RecordUnion(number);

        /// <summary>
        /// Gets all cases in the union.
        /// </summary>
        /// <returns>A readonly list of types.</returns>
        public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
            = new global::System.Type[] { typeof(global::Sundew.DiscriminatedUnions.Development.Tester.RecordUnion) };
    }
}
