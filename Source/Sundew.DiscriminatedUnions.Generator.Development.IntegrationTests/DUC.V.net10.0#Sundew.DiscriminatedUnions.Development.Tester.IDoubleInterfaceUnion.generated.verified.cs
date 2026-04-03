//HintName: Sundew.DiscriminatedUnions.Development.Tester.IDoubleInterfaceUnion.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester
{
#pragma warning disable SA1601
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "6.0.0.0")]
    public partial interface IDoubleInterfaceUnion : global::Sundew.DiscriminatedUnions.IDiscriminatedUnion
#pragma warning restore SA1601
    {
        /// <summary>
        /// Gets the DoubleInterfaceUnion1 case.
        /// </summary>
        /// <returns>The DoubleInterfaceUnion1.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.IDoubleInterfaceUnion.DoubleInterfaceUnion1))]
        [global::System.Diagnostics.DebuggerNonUserCode]
        public new static global::Sundew.DiscriminatedUnions.Development.Tester.IDoubleInterfaceUnion _DoubleInterfaceUnion1 { get; }
            = new global::Sundew.DiscriminatedUnions.Development.Tester.IDoubleInterfaceUnion.DoubleInterfaceUnion1();

        /// <summary>
        /// Gets the DoubleInterfaceUnion2 case.
        /// </summary>
        /// <returns>The DoubleInterfaceUnion2.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.IDoubleInterfaceUnion.DoubleInterfaceUnion2))]
        [global::System.Diagnostics.DebuggerNonUserCode]
        public new static global::Sundew.DiscriminatedUnions.Development.Tester.IDoubleInterfaceUnion _DoubleInterfaceUnion2 { get; }
            = new global::Sundew.DiscriminatedUnions.Development.Tester.IDoubleInterfaceUnion.DoubleInterfaceUnion2();

        /// <summary>
        /// Gets all cases in the union.
        /// </summary>
        /// <returns>A readonly list of types.</returns>
        public new static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
            = new global::System.Type[] { typeof(global::Sundew.DiscriminatedUnions.Development.Tester.IDoubleInterfaceUnion.DoubleInterfaceUnion1), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.IDoubleInterfaceUnion.DoubleInterfaceUnion2) };

        /// <summary>
        /// Gets all cases in the union.
        /// </summary>
        /// <returns>A readonly list of types.</returns>
        static global::System.Collections.Generic.IReadOnlyList<global::System.Type> global::Sundew.DiscriminatedUnions.IDiscriminatedUnion.Cases => Cases;
    }
}
