//HintName: Sundew.DiscriminatedUnions.Tester.NestedGenericUnion{TItem}.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.3.0.0")]
    public partial record NestedGenericUnion<TItem>
#pragma warning restore SA1601
    {
        /// <summary>
        /// Factory method for the Target case.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>A new Target.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.NestedGenericUnion<>.Target))]
        public static global::Sundew.DiscriminatedUnions.Tester.NestedGenericUnion<TItem> _Target(TItem item)
            => new global::Sundew.DiscriminatedUnions.Tester.NestedGenericUnion<TItem>.Target(item);

        /// <summary>
        /// Factory method for the TargetList case.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>A new TargetList.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.NestedGenericUnion<>.TargetList))]
        public static global::Sundew.DiscriminatedUnions.Tester.NestedGenericUnion<TItem> _TargetList(global::System.Collections.Generic.List<TItem> item)
            => new global::Sundew.DiscriminatedUnions.Tester.NestedGenericUnion<TItem>.TargetList(item);
    }
}
