//HintName: Sundew.DiscriminatedUnions.Development.Tester.InvocationExpressionBase.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.3.0.0")]
    internal partial record InvocationExpressionBase
#pragma warning restore SA1601
    {
        /// <summary>
        /// Factory method for the ArrayCreationExpression case.
        /// </summary>
        /// <param name="arrayCreation">The arrayCreation.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>A new ArrayCreationExpression.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.CreationExpression.ArrayCreationExpression))]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.InvocationExpressionBase _ArrayCreationExpression(string arrayCreation, global::System.Collections.Generic.IReadOnlyList<global::System.Linq.Expressions.Expression> arguments)
            => new global::Sundew.DiscriminatedUnions.Development.Tester.CreationExpression.ArrayCreationExpression(arrayCreation, arguments);

        /// <summary>
        /// Factory method for the InvocationExpression case.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>A new InvocationExpression.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.InvocationExpression))]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.InvocationExpressionBase InvocationExpression(global::System.Linq.Expressions.Expression expression, global::System.Collections.Generic.IReadOnlyList<global::System.Linq.Expressions.Expression> arguments)
            => new global::Sundew.DiscriminatedUnions.Development.Tester.InvocationExpression(expression, arguments);
    }
}
