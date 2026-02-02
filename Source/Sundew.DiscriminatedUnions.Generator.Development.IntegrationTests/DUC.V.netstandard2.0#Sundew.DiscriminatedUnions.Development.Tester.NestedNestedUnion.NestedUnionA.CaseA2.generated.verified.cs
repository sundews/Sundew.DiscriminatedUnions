//HintName: Sundew.DiscriminatedUnions.Development.Tester.NestedNestedUnion.NestedUnionA.CaseA2.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester
{
    public partial record NestedNestedUnion
    {
        public partial record NestedUnionA
        {
#pragma warning disable SA1601
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "6.0.0.0")]
            public partial record CaseA2
#pragma warning restore SA1601
            {
                /// <summary>
                /// Gets all cases in the union.
                /// </summary>
                /// <returns>A readonly list of types.</returns>
                public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
                    = new global::System.Type[] { typeof(global::Sundew.DiscriminatedUnions.Development.Tester.NestedNestedUnion.NestedUnionA.CaseA2) };
            }
        }
    }
}
