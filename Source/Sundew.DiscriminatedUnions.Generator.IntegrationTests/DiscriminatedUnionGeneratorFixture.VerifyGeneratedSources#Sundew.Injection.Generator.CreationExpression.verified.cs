//HintName: Sundew.Injection.Generator.CreationExpression.cs
namespace Sundew.Injection.Generator
{
#pragma warning disable SA1601
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "4.0.0.0")]
    internal partial record CreationExpression
#pragma warning restore SA1601
    {
        /// <summary>
        /// Factory method for the ArrayCreationExpression case.
        /// </summary>
        /// <param name="arrayCreation">The arrayCreation.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>A new ArrayCreationExpression.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.Injection.Generator.CreationExpression.ArrayCreationExpression))]
        public new static global::Sundew.Injection.Generator.CreationExpression ArrayCreationExpressionCase(string arrayCreation, global::System.Collections.Generic.IReadOnlyList<global::System.Linq.Expressions.Expression> arguments) => new global::Sundew.Injection.Generator.CreationExpression.ArrayCreationExpression(arrayCreation, arguments);
    }
}
