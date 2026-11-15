//HintName: Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Figure.Circle.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes
{
    partial record Figure
    {
        partial record Circle
        {
            #pragma warning disable CS0108
            /// <summary>
            /// Gets all cases in the union.
            /// </summary>
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
            public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
                = new global::System.Type[] { typeof(global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Figure.Circle) };

            #pragma warning restore CS0108
        }
    }
}
