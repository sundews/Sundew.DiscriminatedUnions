//HintName: Sundew.DiscriminatedUnions.Development.Tester.MixedUnion.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.4.0.0")]
    public partial record MixedUnion
#pragma warning restore SA1601
    {
        /// <summary>
        /// Gets the Case3 case.
        /// </summary>
        /// <returns>The Case3.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.Case3))]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.MixedUnion Case3 { get; }
            = new global::Sundew.DiscriminatedUnions.Development.Tester.Case3();

        /// <summary>
        /// Factory method for the Case1 case.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="isValid">The isValid.</param>
        /// <param name="status">The status.</param>
        /// <param name="number">The number.</param>
        /// <returns>A new Case1.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.MixedUnion.Case1))]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.MixedUnion _Case1(string info, bool isValid = false, global::Sundew.DiscriminatedUnions.Development.Tester.Status status = global::Sundew.DiscriminatedUnions.Development.Tester.Status.On, int number = 3)
            => new global::Sundew.DiscriminatedUnions.Development.Tester.MixedUnion.Case1(info, isValid, status, number);

        /// <summary>
        /// Factory method for the Case2 case.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="optionalList">The optionalList.</param>
        /// <param name="optionalInts">The optionalInts.</param>
        /// <returns>A new Case2.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.Case2))]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.MixedUnion Case2(int? info = 4, global::System.Collections.Generic.List<string?>? optionalList = default, global::System.Collections.Generic.List<int?>? optionalInts = default)
            => new global::Sundew.DiscriminatedUnions.Development.Tester.Case2(info, optionalList, optionalInts);

        /// <summary>
        /// Gets all cases in the union.
        /// </summary>
        /// <returns>A readonly list of types.</returns>
        public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
            = new global::System.Type[] { typeof(global::Sundew.DiscriminatedUnions.Development.Tester.Case3), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.MixedUnion.Case1), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.Case2) };
    }
}
