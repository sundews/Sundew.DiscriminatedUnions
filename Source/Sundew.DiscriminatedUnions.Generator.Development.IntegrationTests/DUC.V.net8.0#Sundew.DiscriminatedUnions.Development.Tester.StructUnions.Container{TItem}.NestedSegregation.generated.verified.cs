//HintName: Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Container{TItem}.NestedSegregation.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.StructUnions
{
    /// <summary>
    /// Contains a list for each case of <c>Nested</c>, with the items that are that case.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
    internal sealed partial class NestedSegregation<TItem>
    {
        internal NestedSegregation(global::System.Collections.Generic.IReadOnlyList<int> int32s, global::System.Collections.Generic.IReadOnlyList<long> int64s, global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Container<TItem>.Wrapper> wrappers)
        {
            this.Int32s = int32s;
            this.Int64s = int64s;
            this.Wrappers = wrappers;
        }

        /// <summary>
        /// Gets the items that are the <c>int</c> case.
        /// </summary>
        public global::System.Collections.Generic.IReadOnlyList<int> Int32s { get; }

        /// <summary>
        /// Gets the items that are the <c>long</c> case.
        /// </summary>
        public global::System.Collections.Generic.IReadOnlyList<long> Int64s { get; }

        /// <summary>
        /// Gets the items that are the <c>Container&lt;TItem&gt;.Wrapper</c> case.
        /// </summary>
        public global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Container<TItem>.Wrapper> Wrappers { get; }
    }
}
