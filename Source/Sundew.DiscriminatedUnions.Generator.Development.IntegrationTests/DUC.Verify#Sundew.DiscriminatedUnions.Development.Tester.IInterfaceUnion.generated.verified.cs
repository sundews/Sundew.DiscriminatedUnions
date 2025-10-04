//HintName: Sundew.DiscriminatedUnions.Development.Tester.IInterfaceUnion.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester
{
#pragma warning disable SA1601
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.3.0.0")]
    public partial interface IInterfaceUnion
#pragma warning restore SA1601
    {
        /// <summary>
        /// Factory method for the RecordUnion case.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>A new RecordUnion.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.RecordUnion))]
        [global::System.Diagnostics.DebuggerNonUserCode]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.IInterfaceUnion RecordUnion(int number)
            => new global::Sundew.DiscriminatedUnions.Development.Tester.RecordUnion(number);
    }
}
