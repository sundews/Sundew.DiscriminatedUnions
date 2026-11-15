//HintName: Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Mammal.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes
{
    partial record Mammal : global::Sundew.DiscriminatedUnions.IDiscriminatedUnion
    {
        #pragma warning disable CS0108
        /// <summary>
        /// Creates a new <c>Dog</c> case of <c>Mammal</c>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The case as <c>Mammal</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static Mammal Dog(string name)
        {
            return new global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Dog(name);
        }

        /// <summary>
        /// Creates a new <c>Cat</c> case of <c>Mammal</c>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The case as <c>Mammal</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static Mammal Cat(string name)
        {
            return new global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Cat(name);
        }

        /// <summary>
        /// Gets all cases in the union.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
            = new global::System.Type[] { typeof(global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Dog), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Cat) };

        #pragma warning restore CS0108
    }
}
