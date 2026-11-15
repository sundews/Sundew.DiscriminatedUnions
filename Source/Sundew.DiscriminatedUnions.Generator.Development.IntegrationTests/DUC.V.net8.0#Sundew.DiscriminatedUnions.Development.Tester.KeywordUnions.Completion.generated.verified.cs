//HintName: Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.Completion.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions
{
    partial union Completion : global::System.IEquatable<Completion>, global::Sundew.DiscriminatedUnions.IDiscriminatedUnion
    {
        /// <summary>
        /// Creates a new <c>Completion</c> that contains a new <c>Completed</c> case.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>The <c>Completion</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static Completion Completed(int code)
        {
            return new Completion(new global::Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.Completed(code));
        }

        /// <summary>
        /// Creates a new <c>Completion</c> that contains a new <c>Failed</c> case.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <returns>The <c>Completion</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static Completion Failed(string reason)
        {
            return new Completion(new global::Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.Failed(reason));
        }

        /// <summary>
        /// Gets all cases in the union.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
            = new global::System.Type[] { typeof(global::Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.Completed), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.Failed) };

        /// <summary>
        /// Indicates whether the left and the right are equal.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns><c>true</c>, if the left and the right are equal, otherwise <c>false</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static bool operator ==(Completion left, Completion right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Indicates whether the left and the right are not equal.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns><c>true</c>, if the left and the right are not equal, otherwise <c>false</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static bool operator !=(Completion left, Completion right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Indicates whether this instance and the other contain equal cases.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c>, if this instance and the other are equal, otherwise <c>false</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool Equals(Completion other)
        {
            return global::System.Object.Equals(this.Value, other.Value);
        }

        /// <inheritdoc/>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override bool Equals(object? obj)
        {
            return obj is Completion other && this.Equals(other);
        }

        /// <inheritdoc/>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override int GetHashCode()
        {
            return this.Value?.GetHashCode() ?? 0;
        }

        /// <inheritdoc/>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override string ToString()
        {
            return this.Value?.ToString() ?? string.Empty;
        }
    }
}
