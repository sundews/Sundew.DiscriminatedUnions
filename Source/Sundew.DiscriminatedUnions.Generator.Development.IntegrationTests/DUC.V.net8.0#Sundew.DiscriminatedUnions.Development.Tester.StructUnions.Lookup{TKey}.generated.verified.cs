//HintName: Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Lookup{TKey}.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.StructUnions
{
    [global::System.Runtime.CompilerServices.UnionAttribute]
    readonly partial struct Lookup<TKey> : global::System.Runtime.CompilerServices.IUnion, global::System.IEquatable<Lookup<TKey>>, global::Sundew.DiscriminatedUnions.IDiscriminatedUnion
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        private static readonly object __boxedMissing = default(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Missing);

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        private readonly global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Found<TKey> __caseFound;

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        private readonly string? __caseString;

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        private readonly byte __tag;

        /// <summary>
        /// Initializes a new instance of the <c>Lookup&lt;TKey&gt;</c> struct that contains the <c>Missing</c> case.
        /// </summary>
        /// <param name="value">The case.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public Lookup(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Missing value)
        {
            this.__tag = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <c>Lookup&lt;TKey&gt;</c> struct that contains the <c>Found&lt;TKey&gt;</c> case.
        /// </summary>
        /// <param name="value">The case.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public Lookup(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Found<TKey> value)
        {
            this.__caseFound = value;
            this.__tag = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <c>Lookup&lt;TKey&gt;</c> struct that contains the <c>string</c> case.
        /// </summary>
        /// <param name="value">The case.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public Lookup(string value)
        {
            this.__caseString = value ?? throw new global::System.ArgumentNullException(nameof(value));
            this.__tag = 2;
        }

        /// <summary>
        /// Gets the <c>Lookup&lt;TKey&gt;</c> that contains the <c>Missing</c> case.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        public static Lookup<TKey> Missing => new Lookup<TKey>(new global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Missing());

        /// <summary>
        /// Creates a new <c>Lookup&lt;TKey&gt;</c> that contains a new <c>Found&lt;TKey&gt;</c> case.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="index">The index.</param>
        /// <returns>The <c>Lookup&lt;TKey&gt;</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static Lookup<TKey> Found(TKey key, int index)
        {
            return new Lookup<TKey>(new global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Found<TKey>(key, index));
        }

        /// <summary>
        /// Gets all cases in the union.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
            = new global::System.Type[] { typeof(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Missing), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Found<TKey>), typeof(string) };

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
                        return this.__caseFound;
                    case 2:
                        return this.__caseString;
                    default:
                        return __boxedMissing;
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
        /// Tries to get the <c>Missing</c> case.
        /// </summary>
        /// <param name="value">The case, if this instance contains it.</param>
        /// <returns><c>true</c>, if this instance contains the case, otherwise <c>false</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool TryGetValue(out global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Missing value)
        {
            value = default(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Missing);
            return this.__tag == 0;
        }

        /// <summary>
        /// Tries to get the <c>Found&lt;TKey&gt;</c> case.
        /// </summary>
        /// <param name="value">The case, if this instance contains it.</param>
        /// <returns><c>true</c>, if this instance contains the case, otherwise <c>false</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool TryGetValue(out global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Found<TKey> value)
        {
            value = this.__caseFound;
            return this.__tag == 1;
        }

        /// <summary>
        /// Tries to get the <c>string</c> case.
        /// </summary>
        /// <param name="value">The case, if this instance contains it.</param>
        /// <returns><c>true</c>, if this instance contains the case, otherwise <c>false</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool TryGetValue(out string value)
        {
            value = this.__caseString!;
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
        public static bool operator ==(Lookup<TKey> left, Lookup<TKey> right)
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
        public static bool operator !=(Lookup<TKey> left, Lookup<TKey> right)
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
        public bool Equals(Lookup<TKey> other)
        {
            if (this.__tag != other.__tag)
            {
                return false;
            }

            switch (this.__tag)
            {
                case 1:
                    return global::System.Collections.Generic.EqualityComparer<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Found<TKey>>.Default.Equals(this.__caseFound, other.__caseFound);
                case 2:
                    return global::System.Collections.Generic.EqualityComparer<string>.Default.Equals(this.__caseString!, other.__caseString!);
                default:
                    return true;
            }
        }

        /// <inheritdoc/>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override bool Equals(object? obj)
        {
            return obj is Lookup<TKey> other && this.Equals(other);
        }

        /// <inheritdoc/>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override int GetHashCode()
        {
            switch (this.__tag)
            {
                case 1:
                    return unchecked((1 * 397) ^ global::System.Collections.Generic.EqualityComparer<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Found<TKey>>.Default.GetHashCode(this.__caseFound));
                case 2:
                    return unchecked((2 * 397) ^ global::System.Collections.Generic.EqualityComparer<string>.Default.GetHashCode(this.__caseString!));
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
                    return this.__caseFound.ToString() ?? string.Empty;
                case 2:
                    return this.__caseString!.ToString() ?? string.Empty;
                default:
                    return default(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Missing).ToString() ?? string.Empty;
            }
        }
    }
}
