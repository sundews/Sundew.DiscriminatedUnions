//HintName: Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Measurement.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.StructUnions
{
    [global::System.Runtime.CompilerServices.UnionAttribute]
    readonly partial struct Measurement : global::System.Runtime.CompilerServices.IUnion, global::System.IEquatable<Measurement>, global::Sundew.DiscriminatedUnions.IDiscriminatedUnion
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        private static readonly object __boxedUnknown = default(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Unknown);

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        private readonly __UnmanagedCases __unmanaged;

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        private readonly byte __tag;

        /// <summary>
        /// Initializes a new instance of the <c>Measurement</c> struct that contains the <c>Unknown</c> case.
        /// </summary>
        /// <param name="value">The case.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public Measurement(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Unknown value)
        {
            this.__tag = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <c>Measurement</c> struct that contains the <c>Quantity&lt;int&gt;</c> case.
        /// </summary>
        /// <param name="value">The case.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public Measurement(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Quantity<int> value)
        {
            this.__unmanaged = new __UnmanagedCases { Quantity1 = value };
            this.__tag = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <c>Measurement</c> struct that contains the <c>Quantity&lt;double&gt;</c> case.
        /// </summary>
        /// <param name="value">The case.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public Measurement(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Quantity<double> value)
        {
            this.__unmanaged = new __UnmanagedCases { Quantity2 = value };
            this.__tag = 2;
        }

        /// <summary>
        /// Gets the <c>Measurement</c> that contains the <c>Unknown</c> case.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        public static Measurement Unknown => new Measurement(new global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Unknown());

        /// <summary>
        /// Creates a new <c>Measurement</c> that contains a new <c>Quantity&lt;int&gt;</c> case.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The <c>Measurement</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static Measurement Quantity(int value)
        {
            return new Measurement(new global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Quantity<int>(value));
        }

        /// <summary>
        /// Creates a new <c>Measurement</c> that contains a new <c>Quantity&lt;double&gt;</c> case.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The <c>Measurement</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static Measurement Quantity(double value)
        {
            return new Measurement(new global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Quantity<double>(value));
        }

        /// <summary>
        /// Gets all cases in the union.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
            = new global::System.Type[] { typeof(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Unknown), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Quantity<int>), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Quantity<double>) };

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
                        return this.__unmanaged.Quantity1;
                    case 2:
                        return this.__unmanaged.Quantity2;
                    default:
                        return __boxedUnknown;
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
        /// Tries to get the <c>Unknown</c> case.
        /// </summary>
        /// <param name="value">The case, if this instance contains it.</param>
        /// <returns><c>true</c>, if this instance contains the case, otherwise <c>false</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool TryGetValue(out global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Unknown value)
        {
            value = default(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Unknown);
            return this.__tag == 0;
        }

        /// <summary>
        /// Tries to get the <c>Quantity&lt;int&gt;</c> case.
        /// </summary>
        /// <param name="value">The case, if this instance contains it.</param>
        /// <returns><c>true</c>, if this instance contains the case, otherwise <c>false</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool TryGetValue(out global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Quantity<int> value)
        {
            if (this.__tag == 1)
            {
                value = this.__unmanaged.Quantity1;
                return true;
            }

            value = default!;
            return false;
        }

        /// <summary>
        /// Tries to get the <c>Quantity&lt;double&gt;</c> case.
        /// </summary>
        /// <param name="value">The case, if this instance contains it.</param>
        /// <returns><c>true</c>, if this instance contains the case, otherwise <c>false</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool TryGetValue(out global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Quantity<double> value)
        {
            if (this.__tag == 2)
            {
                value = this.__unmanaged.Quantity2;
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
        public static bool operator ==(Measurement left, Measurement right)
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
        public static bool operator !=(Measurement left, Measurement right)
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
        public bool Equals(Measurement other)
        {
            if (this.__tag != other.__tag)
            {
                return false;
            }

            switch (this.__tag)
            {
                case 1:
                    return global::System.Collections.Generic.EqualityComparer<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Quantity<int>>.Default.Equals(this.__unmanaged.Quantity1, other.__unmanaged.Quantity1);
                case 2:
                    return global::System.Collections.Generic.EqualityComparer<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Quantity<double>>.Default.Equals(this.__unmanaged.Quantity2, other.__unmanaged.Quantity2);
                default:
                    return true;
            }
        }

        /// <inheritdoc/>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override bool Equals(object? obj)
        {
            return obj is Measurement other && this.Equals(other);
        }

        /// <inheritdoc/>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override int GetHashCode()
        {
            switch (this.__tag)
            {
                case 1:
                    return unchecked((1 * 397) ^ global::System.Collections.Generic.EqualityComparer<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Quantity<int>>.Default.GetHashCode(this.__unmanaged.Quantity1));
                case 2:
                    return unchecked((2 * 397) ^ global::System.Collections.Generic.EqualityComparer<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Quantity<double>>.Default.GetHashCode(this.__unmanaged.Quantity2));
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
                    return this.__unmanaged.Quantity1.ToString() ?? string.Empty;
                case 2:
                    return this.__unmanaged.Quantity2.ToString() ?? string.Empty;
                default:
                    return default(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Unknown).ToString() ?? string.Empty;
            }
        }

        [global::System.Runtime.InteropServices.StructLayoutAttribute(global::System.Runtime.InteropServices.LayoutKind.Explicit)]
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        private struct __UnmanagedCases
        {
            [global::System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Quantity<int> Quantity1;

            [global::System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Quantity<double> Quantity2;
        }
    }
}
