//HintName: Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Animal.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes
{
    partial record Animal : global::Sundew.DiscriminatedUnions.IDiscriminatedUnion
    {
        /// <summary>
        /// Creates a new <c>Dog</c> case of <c>Animal</c>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The case as <c>Animal</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static Animal Dog(string name)
        {
            return new global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Dog(name);
        }

        /// <summary>
        /// Creates a new <c>Cat</c> case of <c>Animal</c>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The case as <c>Animal</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static Animal Cat(string name)
        {
            return new global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Cat(name);
        }

        /// <summary>
        /// Gets the <c>Fish</c> case of <c>Animal</c>.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        public static Animal Fish { get; }
            = new global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Fish();

        /// <summary>
        /// Gets all cases in the union.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
            = new global::System.Type[] { typeof(global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Dog), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Cat), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Fish) };
    }
}
