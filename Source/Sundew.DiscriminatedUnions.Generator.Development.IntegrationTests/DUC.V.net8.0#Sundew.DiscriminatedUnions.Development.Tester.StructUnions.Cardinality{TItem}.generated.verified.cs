//HintName: Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Cardinality{TItem}.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.StructUnions
{
    [global::System.Runtime.CompilerServices.UnionAttribute]
    readonly partial struct Cardinality<TItem> : global::System.Runtime.CompilerServices.IUnion, global::System.IEquatable<Cardinality<TItem>>, global::Sundew.DiscriminatedUnions.IDiscriminatedUnion
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        private static readonly object __boxedEmpty = default(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Empty<TItem>);

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        private readonly global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Single<TItem> __caseSingle;

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        private readonly global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Multiple<TItem> __caseMultiple;

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        private readonly byte __tag;

        /// <summary>
        /// Initializes a new instance of the <c>Cardinality&lt;TItem&gt;</c> struct that contains the <c>Empty&lt;TItem&gt;</c> case.
        /// </summary>
        /// <param name="value">The case.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public Cardinality(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Empty<TItem> value)
        {
            this.__tag = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <c>Cardinality&lt;TItem&gt;</c> struct that contains the <c>Single&lt;TItem&gt;</c> case.
        /// </summary>
        /// <param name="value">The case.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public Cardinality(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Single<TItem> value)
        {
            this.__caseSingle = value;
            this.__tag = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <c>Cardinality&lt;TItem&gt;</c> struct that contains the <c>Multiple&lt;TItem&gt;</c> case.
        /// </summary>
        /// <param name="value">The case.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public Cardinality(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Multiple<TItem> value)
        {
            this.__caseMultiple = value;
            this.__tag = 2;
        }

        /// <summary>
        /// Gets all cases in the union.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
            = new global::System.Type[] { typeof(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Empty<TItem>), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Single<TItem>), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Multiple<TItem>) };

        /// <summary>
        /// Gets the case this instance contains. Reading it boxes a case that is a struct, so prefer pattern matching.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public object? Value
        {
            get
            {
                switch (this.__tag)
                {
                    case 1:
                        return this.__caseSingle;
                    case 2:
                        return this.__caseMultiple;
                    default:
                        return __boxedEmpty;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance contains a case. It always does, because the default value is a case.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool HasValue => true;

        /// <summary>
        /// Tries to get the <c>Empty&lt;TItem&gt;</c> case.
        /// </summary>
        /// <param name="value">The case, if this instance contains it.</param>
        /// <returns><c>true</c>, if this instance contains the case, otherwise <c>false</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool TryGetValue(out global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Empty<TItem> value)
        {
            value = default(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Empty<TItem>);
            return this.__tag == 0;
        }

        /// <summary>
        /// Tries to get the <c>Single&lt;TItem&gt;</c> case.
        /// </summary>
        /// <param name="value">The case, if this instance contains it.</param>
        /// <returns><c>true</c>, if this instance contains the case, otherwise <c>false</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool TryGetValue(out global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Single<TItem> value)
        {
            value = this.__caseSingle;
            return this.__tag == 1;
        }

        /// <summary>
        /// Tries to get the <c>Multiple&lt;TItem&gt;</c> case.
        /// </summary>
        /// <param name="value">The case, if this instance contains it.</param>
        /// <returns><c>true</c>, if this instance contains the case, otherwise <c>false</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool TryGetValue(out global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Multiple<TItem> value)
        {
            value = this.__caseMultiple;
            return this.__tag == 2;
        }

        /// <summary>
        /// Indicates whether the left and the right are equal.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns><c>true</c>, if the left and the right are equal, otherwise <c>false</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static bool operator ==(Cardinality<TItem> left, Cardinality<TItem> right)
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
        public static bool operator !=(Cardinality<TItem> left, Cardinality<TItem> right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Indicates whether this instance and the other contain the same case with equal values.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c>, if this instance and the other are equal, otherwise <c>false</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool Equals(Cardinality<TItem> other)
        {
            if (this.__tag != other.__tag)
            {
                return false;
            }

            switch (this.__tag)
            {
                case 1:
                    return global::System.Collections.Generic.EqualityComparer<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Single<TItem>>.Default.Equals(this.__caseSingle, other.__caseSingle);
                case 2:
                    return global::System.Collections.Generic.EqualityComparer<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Multiple<TItem>>.Default.Equals(this.__caseMultiple, other.__caseMultiple);
                default:
                    return true;
            }
        }

        /// <inheritdoc/>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override bool Equals(object? obj)
        {
            return obj is Cardinality<TItem> other && this.Equals(other);
        }

        /// <inheritdoc/>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override int GetHashCode()
        {
            switch (this.__tag)
            {
                case 1:
                    return unchecked((1 * 397) ^ global::System.Collections.Generic.EqualityComparer<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Single<TItem>>.Default.GetHashCode(this.__caseSingle));
                case 2:
                    return unchecked((2 * 397) ^ global::System.Collections.Generic.EqualityComparer<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Multiple<TItem>>.Default.GetHashCode(this.__caseMultiple));
                default:
                    return this.__tag;
            }
        }

        /// <inheritdoc/>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override string ToString()
        {
            switch (this.__tag)
            {
                case 1:
                    return this.__caseSingle.ToString() ?? string.Empty;
                case 2:
                    return this.__caseMultiple.ToString() ?? string.Empty;
                default:
                    return default(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Empty<TItem>).ToString() ?? string.Empty;
            }
        }
    }
}
