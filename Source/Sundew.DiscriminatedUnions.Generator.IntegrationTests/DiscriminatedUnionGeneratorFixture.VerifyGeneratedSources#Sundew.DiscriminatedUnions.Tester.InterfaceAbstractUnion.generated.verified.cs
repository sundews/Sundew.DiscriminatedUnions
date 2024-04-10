//HintName: Sundew.DiscriminatedUnions.Tester.InterfaceAbstractUnion.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.3.0.0")]
    public partial record InterfaceAbstractUnion
#pragma warning restore SA1601
    {
        /// <summary>
        /// Factory method for the RecordUnion case.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>A new RecordUnion.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.RecordUnion))]
        public static global::Sundew.DiscriminatedUnions.Tester.InterfaceAbstractUnion RecordUnion(int number)
            => new global::Sundew.DiscriminatedUnions.Tester.RecordUnion(number);
    }
}
