//HintName: Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Attempt{TValue,TError}.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes
{
    partial record Attempt<TValue, TError> : global::Sundew.DiscriminatedUnions.IDiscriminatedUnion
    {
        /// <summary>
        /// Creates a new <c>Attempt&lt;TValue, TError&gt;.Succeeded</c> case of <c>Attempt&lt;TValue, TError&gt;</c>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The case as <c>Attempt&lt;TValue, TError&gt;</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static Attempt<TValue, TError> _Succeeded(TValue value)
        {
            return new global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Attempt<TValue, TError>.Succeeded(value);
        }

        /// <summary>
        /// Creates a new <c>Faulted&lt;TValue, TError&gt;</c> case of <c>Attempt&lt;TValue, TError&gt;</c>.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns>The case as <c>Attempt&lt;TValue, TError&gt;</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static Attempt<TValue, TError> Faulted(TError error)
        {
            return new global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Faulted<TValue, TError>(error);
        }

        /// <summary>
        /// Gets all cases in the union.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
            = new global::System.Type[] { typeof(global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Attempt<TValue, TError>.Succeeded), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Faulted<TValue, TError>) };
    }
}
