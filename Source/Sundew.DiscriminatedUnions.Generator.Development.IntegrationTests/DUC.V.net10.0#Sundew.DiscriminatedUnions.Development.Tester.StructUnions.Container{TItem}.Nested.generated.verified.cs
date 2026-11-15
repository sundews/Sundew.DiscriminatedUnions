//HintName: Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Container{TItem}.Nested.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.StructUnions
{
    partial class Container<TItem>
    {
        [global::System.Runtime.CompilerServices.UnionAttribute]
        readonly partial struct Nested : global::System.Runtime.CompilerServices.IUnion, global::System.IEquatable<Nested>, global::Sundew.DiscriminatedUnions.IDiscriminatedUnion
        {
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
            private readonly global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.__Container_1_Nested_UnmanagedCases __unmanaged;

            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
            private readonly global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Container<TItem>.Wrapper __caseWrapper;

            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
            private readonly byte __tag;

            /// <summary>
            /// Initializes a new instance of the <c>Nested</c> struct that contains the <c>int</c> case.
            /// </summary>
            /// <param name="value">The case.</param>
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
            [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
            public Nested(int value)
            {
                this.__unmanaged = new global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.__Container_1_Nested_UnmanagedCases { Int32 = value };
                this.__tag = 1;
            }

            /// <summary>
            /// Initializes a new instance of the <c>Nested</c> struct that contains the <c>long</c> case.
            /// </summary>
            /// <param name="value">The case.</param>
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
            [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
            public Nested(long value)
            {
                this.__unmanaged = new global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.__Container_1_Nested_UnmanagedCases { Int64 = value };
                this.__tag = 2;
            }

            /// <summary>
            /// Initializes a new instance of the <c>Nested</c> struct that contains the <c>Container&lt;TItem&gt;.Wrapper</c> case.
            /// </summary>
            /// <param name="value">The case.</param>
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
            [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
            public Nested(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Container<TItem>.Wrapper value)
            {
                this.__caseWrapper = value;
                this.__tag = 3;
            }

            /// <summary>
            /// Creates a new <c>Nested</c> that contains a new <c>Container&lt;TItem&gt;.Wrapper</c> case.
            /// </summary>
            /// <param name="item">The item.</param>
            /// <returns>The <c>Nested</c>.</returns>
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
            [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
            internal static Nested Wrapper(TItem item)
            {
                return new Nested(new global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Container<TItem>.Wrapper(item));
            }

            /// <summary>
            /// Gets all cases in the union.
            /// </summary>
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
            public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
                = new global::System.Type[] { typeof(int), typeof(long), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Container<TItem>.Wrapper) };

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
                            return this.__unmanaged.Int32;
                        case 2:
                            return this.__unmanaged.Int64;
                        case 3:
                            return this.__caseWrapper;
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
            /// Tries to get the <c>int</c> case.
            /// </summary>
            /// <param name="value">The case, if this instance contains it.</param>
            /// <returns><c>true</c>, if this instance contains the case, otherwise <c>false</c>.</returns>
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
            [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
            public bool TryGetValue(out int value)
            {
                if (this.__tag == 1)
                {
                    value = this.__unmanaged.Int32;
                    return true;
                }

                value = default!;
                return false;
            }

            /// <summary>
            /// Tries to get the <c>long</c> case.
            /// </summary>
            /// <param name="value">The case, if this instance contains it.</param>
            /// <returns><c>true</c>, if this instance contains the case, otherwise <c>false</c>.</returns>
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
            [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
            public bool TryGetValue(out long value)
            {
                if (this.__tag == 2)
                {
                    value = this.__unmanaged.Int64;
                    return true;
                }

                value = default!;
                return false;
            }

            /// <summary>
            /// Tries to get the <c>Container&lt;TItem&gt;.Wrapper</c> case.
            /// </summary>
            /// <param name="value">The case, if this instance contains it.</param>
            /// <returns><c>true</c>, if this instance contains the case, otherwise <c>false</c>.</returns>
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
            [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
            public bool TryGetValue(out global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Container<TItem>.Wrapper value)
            {
                value = this.__caseWrapper;
                return this.__tag == 3;
            }

            /// <summary>
            /// Indicates whether the left and the right are equal.
            /// </summary>
            /// <param name="left">The left.</param>
            /// <param name="right">The right.</param>
            /// <returns><c>true</c>, if the left and the right are equal, otherwise <c>false</c>.</returns>
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
            [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
            public static bool operator ==(Nested left, Nested right)
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
            public static bool operator !=(Nested left, Nested right)
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
            public bool Equals(Nested other)
            {
                if (this.__tag != other.__tag)
                {
                    return false;
                }

                switch (this.__tag)
                {
                    case 1:
                        return global::System.Collections.Generic.EqualityComparer<int>.Default.Equals(this.__unmanaged.Int32, other.__unmanaged.Int32);
                    case 2:
                        return global::System.Collections.Generic.EqualityComparer<long>.Default.Equals(this.__unmanaged.Int64, other.__unmanaged.Int64);
                    case 3:
                        return global::System.Collections.Generic.EqualityComparer<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Container<TItem>.Wrapper>.Default.Equals(this.__caseWrapper, other.__caseWrapper);
                    default:
                        return true;
                }
            }

            /// <inheritdoc/>
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
            [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
            public override bool Equals(object? obj)
            {
                return obj is Nested other && this.Equals(other);
            }

            /// <inheritdoc/>
            [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
            [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
            public override int GetHashCode()
            {
                switch (this.__tag)
                {
                    case 1:
                        return unchecked((1 * 397) ^ global::System.Collections.Generic.EqualityComparer<int>.Default.GetHashCode(this.__unmanaged.Int32));
                    case 2:
                        return unchecked((2 * 397) ^ global::System.Collections.Generic.EqualityComparer<long>.Default.GetHashCode(this.__unmanaged.Int64));
                    case 3:
                        return unchecked((3 * 397) ^ global::System.Collections.Generic.EqualityComparer<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Container<TItem>.Wrapper>.Default.GetHashCode(this.__caseWrapper));
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
                        return this.__unmanaged.Int32.ToString() ?? string.Empty;
                    case 2:
                        return this.__unmanaged.Int64.ToString() ?? string.Empty;
                    case 3:
                        return this.__caseWrapper.ToString() ?? string.Empty;
                    default:
                        return string.Empty;
                }
            }
        }
    }

    /// <summary>
    /// Stores the cases of <c>Nested</c> that contain no references and do not depend on a type parameter, so that they can overlap.
    /// It is a type of its own, because the runtime does not allow a generic type to lay out its fields explicitly.
    /// </summary>
    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
    [global::System.Runtime.InteropServices.StructLayoutAttribute(global::System.Runtime.InteropServices.LayoutKind.Explicit)]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
    internal struct __Container_1_Nested_UnmanagedCases
    {
        [global::System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public int Int32;

        [global::System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public long Int64;
    }
}
