//HintName: Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.MammalSegregation.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes
{
    /// <summary>
    /// Contains a list for each case of <c>Mammal</c>, with the items that are that case.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
    public sealed partial class MammalSegregation
    {
        internal MammalSegregation(global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Dog> dogs, global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Cat> cats)
        {
            this.Dogs = dogs;
            this.Cats = cats;
        }

        /// <summary>
        /// Gets the items that are the <c>Dog</c> case.
        /// </summary>
        public global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Dog> Dogs { get; }

        /// <summary>
        /// Gets the items that are the <c>Cat</c> case.
        /// </summary>
        public global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Cat> Cats { get; }
    }
}
