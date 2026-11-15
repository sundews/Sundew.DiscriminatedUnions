//HintName: Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.AnimalSegregation.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes
{
    /// <summary>
    /// Contains a list for each case of <c>Animal</c>, with the items that are that case.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
    public sealed partial class AnimalSegregation
    {
        internal AnimalSegregation(global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Dog> dogs, global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Cat> cats, global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Fish> fishs)
        {
            this.Dogs = dogs;
            this.Cats = cats;
            this.Fishs = fishs;
        }

        /// <summary>
        /// Gets the items that are the <c>Dog</c> case.
        /// </summary>
        public global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Dog> Dogs { get; }

        /// <summary>
        /// Gets the items that are the <c>Cat</c> case.
        /// </summary>
        public global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Cat> Cats { get; }

        /// <summary>
        /// Gets the items that are the <c>Fish</c> case.
        /// </summary>
        public global::System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Fish> Fishs { get; }
    }
}
