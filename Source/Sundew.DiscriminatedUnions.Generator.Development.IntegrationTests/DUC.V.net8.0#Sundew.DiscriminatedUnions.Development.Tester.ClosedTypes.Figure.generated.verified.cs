//HintName: Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Figure.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes
{
    partial record Figure : global::Sundew.DiscriminatedUnions.IDiscriminatedUnion
    {
        /// <summary>
        /// Creates a new <c>Figure.Circle</c> case of <c>Figure</c>.
        /// </summary>
        /// <param name="radius">The radius.</param>
        /// <returns>The case as <c>Figure</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static Figure _Circle(double radius)
        {
            return new global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Figure.Circle(radius);
        }

        /// <summary>
        /// Gets the <c>Figure.Dot</c> case of <c>Figure</c>.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        public static Figure _Dot { get; }
            = new global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Figure.Dot();

        /// <summary>
        /// Creates a new <c>Box</c> case of <c>Figure</c>.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>The case as <c>Figure</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static Figure Box(double width, double height)
        {
            return new global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Box(width, height);
        }

        /// <summary>
        /// Gets all cases in the union.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
            = new global::System.Type[] { typeof(global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Figure.Circle), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Figure.Dot), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Box) };
    }
}
