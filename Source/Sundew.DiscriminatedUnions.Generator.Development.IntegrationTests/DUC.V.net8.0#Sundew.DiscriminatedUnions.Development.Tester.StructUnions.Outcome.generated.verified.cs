//HintName: Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Outcome.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.StructUnions
{
    [global::System.Runtime.CompilerServices.UnionAttribute]
    readonly partial struct Outcome : global::System.Runtime.CompilerServices.IUnion, global::System.IEquatable<Outcome>, global::Sundew.DiscriminatedUnions.IDiscriminatedUnion
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        private readonly global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Success __caseSuccess;

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        private readonly object? __reference;

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        private readonly byte __tag;

        /// <summary>
        /// Initializes a new instance of the <c>Outcome</c> struct that contains the <c>Success</c> case.
        /// </summary>
        /// <param name="value">The case.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public Outcome(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Success value)
        {
            this.__caseSuccess = value;
            this.__tag = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <c>Outcome</c> struct that contains the <c>string</c> case.
        /// </summary>
        /// <param name="value">The case.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public Outcome(string value)
        {
            this.__reference = value;
            this.__tag = value is null ? (byte)0 : (byte)2;
        }

        /// <summary>
        /// Initializes a new instance of the <c>Outcome</c> struct that contains the <c>Exception</c> case.
        /// </summary>
        /// <param name="value">The case.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public Outcome(global::System.Exception value)
        {
            this.__reference = value;
            this.__tag = value is null ? (byte)0 : (byte)3;
        }

        /// <summary>
        /// Creates a new <c>Outcome</c> that contains a new <c>Success</c> case.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>The <c>Outcome</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static Outcome Success(int code)
        {
            return new Outcome(new global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Success(code));
        }

        /// <summary>
        /// Gets all cases in the union.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
            = new global::System.Type[] { typeof(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Success), typeof(string), typeof(global::System.Exception) };

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
                        return this.__caseSuccess;
                    case 2:
                        return this.__reference;
                    case 3:
                        return this.__reference;
                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance contains a case. The default value does not.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool HasValue => this.__tag != 0;

        /// <summary>
        /// Tries to get the <c>Success</c> case.
        /// </summary>
        /// <param name="value">The case, if this instance contains it.</param>
        /// <returns><c>true</c>, if this instance contains the case, otherwise <c>false</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool TryGetValue(out global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Success value)
        {
            value = this.__caseSuccess;
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
            if (this.__tag == 2)
            {
                value = ((string)this.__reference!);
                return true;
            }

            value = default!;
            return false;
        }

        /// <summary>
        /// Tries to get the <c>Exception</c> case.
        /// </summary>
        /// <param name="value">The case, if this instance contains it.</param>
        /// <returns><c>true</c>, if this instance contains the case, otherwise <c>false</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool TryGetValue(out global::System.Exception value)
        {
            if (this.__tag == 3)
            {
                value = ((global::System.Exception)this.__reference!);
                return true;
            }

            value = default!;
            return false;
        }

        /// <summary>
        /// Indicates whether the left and the right are equal.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns><c>true</c>, if the left and the right are equal, otherwise <c>false</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static bool operator ==(Outcome left, Outcome right)
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
        public static bool operator !=(Outcome left, Outcome right)
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
        public bool Equals(Outcome other)
        {
            if (this.__tag != other.__tag)
            {
                return false;
            }

            switch (this.__tag)
            {
                case 1:
                    return global::System.Collections.Generic.EqualityComparer<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Success>.Default.Equals(this.__caseSuccess, other.__caseSuccess);
                case 2:
                    return global::System.Collections.Generic.EqualityComparer<string>.Default.Equals(((string)this.__reference!), ((string)other.__reference!));
                case 3:
                    return global::System.Collections.Generic.EqualityComparer<global::System.Exception>.Default.Equals(((global::System.Exception)this.__reference!), ((global::System.Exception)other.__reference!));
                default:
                    return true;
            }
        }

        /// <inheritdoc/>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override bool Equals(object? obj)
        {
            return obj is Outcome other && this.Equals(other);
        }

        /// <inheritdoc/>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override int GetHashCode()
        {
            switch (this.__tag)
            {
                case 1:
                    return unchecked((1 * 397) ^ global::System.Collections.Generic.EqualityComparer<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Success>.Default.GetHashCode(this.__caseSuccess));
                case 2:
                    return unchecked((2 * 397) ^ global::System.Collections.Generic.EqualityComparer<string>.Default.GetHashCode(((string)this.__reference!)));
                case 3:
                    return unchecked((3 * 397) ^ global::System.Collections.Generic.EqualityComparer<global::System.Exception>.Default.GetHashCode(((global::System.Exception)this.__reference!)));
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
                    return this.__caseSuccess.ToString() ?? string.Empty;
                case 2:
                    return ((string)this.__reference!).ToString() ?? string.Empty;
                case 3:
                    return ((global::System.Exception)this.__reference!).ToString() ?? string.Empty;
                default:
                    return string.Empty;
            }
        }
    }
}
