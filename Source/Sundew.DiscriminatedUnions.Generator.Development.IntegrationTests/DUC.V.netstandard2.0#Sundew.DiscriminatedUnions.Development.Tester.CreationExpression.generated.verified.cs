//HintName: Sundew.DiscriminatedUnions.Development.Tester.CreationExpression.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.4.0.0")]
    internal partial record CreationExpression
#pragma warning restore SA1601
    {
        /// <summary>
        /// Factory method for the ArrayCreationExpression case.
        /// </summary>
        /// <param name="arrayCreation">The arrayCreation.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>A new ArrayCreationExpression.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.CreationExpression.ArrayCreationExpression))]
        public new static global::Sundew.DiscriminatedUnions.Development.Tester.CreationExpression _ArrayCreationExpression(string arrayCreation, global::System.Collections.Generic.IReadOnlyList<global::System.Linq.Expressions.Expression> arguments)
            => new global::Sundew.DiscriminatedUnions.Development.Tester.CreationExpression.ArrayCreationExpression(arrayCreation, arguments);

        /// <summary>
        /// Gets all cases in the union.
        /// </summary>
        /// <returns>A readonly list of types.</returns>
        public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
            = new global::System.Type[] { typeof(global::Sundew.DiscriminatedUnions.Development.Tester.CreationExpression.ArrayCreationExpression) };
    }
}
