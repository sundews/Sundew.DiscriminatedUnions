//HintName: Sundew.DiscriminatedUnions.Development.Tester.NestedNestedUnion.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "6.0.0.0")]
    public partial record NestedNestedUnion : global::Sundew.DiscriminatedUnions.IDiscriminatedUnion
#pragma warning restore SA1601
    {
        /// <summary>
        /// Factory method for the CaseA1 case.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A new CaseA1.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.NestedNestedUnion.NestedUnionA.CaseA1))]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.NestedNestedUnion _CaseA1(int value)
            => new global::Sundew.DiscriminatedUnions.Development.Tester.NestedNestedUnion.NestedUnionA.CaseA1(value);

        /// <summary>
        /// Factory method for the CaseA2 case.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>A new CaseA2.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.NestedNestedUnion.NestedUnionA.CaseA2))]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.NestedNestedUnion _CaseA2(string text)
            => new global::Sundew.DiscriminatedUnions.Development.Tester.NestedNestedUnion.NestedUnionA.CaseA2(text);

        /// <summary>
        /// Gets all cases in the union.
        /// </summary>
        /// <returns>A readonly list of types.</returns>
        public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
            = new global::System.Type[] { typeof(global::Sundew.DiscriminatedUnions.Development.Tester.NestedNestedUnion.NestedUnionA.CaseA1), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.NestedNestedUnion.NestedUnionA.CaseA2) };
    }
}
