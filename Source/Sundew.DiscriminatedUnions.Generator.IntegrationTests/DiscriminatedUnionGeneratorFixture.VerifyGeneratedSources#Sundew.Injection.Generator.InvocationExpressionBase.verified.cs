//HintName: Sundew.Injection.Generator.InvocationExpressionBase.cs
namespace Sundew.Injection.Generator
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "4.0.0.0")]
    internal partial record InvocationExpressionBase
#pragma warning restore SA1601
    {
        /// <summary>
        /// Factory method for the ArrayCreationExpression case.
        /// </summary>
        /// <param name="arrayCreation">The arrayCreation.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>A new ArrayCreationExpression.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.Injection.Generator.CreationExpression.ArrayCreationExpression))]
        public static global::Sundew.Injection.Generator.InvocationExpressionBase ArrayCreationExpressionCase(string arrayCreation, global::System.Collections.Generic.IReadOnlyList<global::System.Linq.Expressions.Expression> arguments) => new global::Sundew.Injection.Generator.CreationExpression.ArrayCreationExpression(arrayCreation, arguments);

        /// <summary>
        /// Factory method for the InvocationExpression case.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>A new InvocationExpression.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.Injection.Generator.InvocationExpression))]
        public static global::Sundew.Injection.Generator.InvocationExpressionBase InvocationExpression(global::System.Linq.Expressions.Expression expression, global::System.Collections.Generic.IReadOnlyList<global::System.Linq.Expressions.Expression> arguments) => new global::Sundew.Injection.Generator.InvocationExpression(expression, arguments);
    }
}
