//HintName: Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Shape.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.StructUnions
{
    [global::System.Runtime.CompilerServices.UnionAttribute]
    readonly partial struct Shape : global::System.Runtime.CompilerServices.IUnion, global::System.IEquatable<Shape>, global::Sundew.DiscriminatedUnions.IDiscriminatedUnion
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        private static readonly object __boxedPoint = default(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Point);

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        private readonly __UnmanagedCases __unmanaged;

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        private readonly byte __tag;

        /// <summary>
        /// Initializes a new instance of the <c>Shape</c> struct that contains the <c>Circle</c> case.
        /// </summary>
        /// <param name="value">The case.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public Shape(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Circle value)
        {
            this.__unmanaged = new __UnmanagedCases { Circle = value };
            this.__tag = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <c>Shape</c> struct that contains the <c>Rectangle</c> case.
        /// </summary>
        /// <param name="value">The case.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public Shape(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Rectangle value)
        {
            this.__unmanaged = new __UnmanagedCases { Rectangle = value };
            this.__tag = 2;
        }

        /// <summary>
        /// Initializes a new instance of the <c>Shape</c> struct that contains the <c>Point</c> case.
        /// </summary>
        /// <param name="value">The case.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public Shape(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Point value)
        {
            this.__tag = 3;
        }

        /// <summary>
        /// Creates a new <c>Shape</c> that contains a new <c>Circle</c> case.
        /// </summary>
        /// <param name="radius">The radius.</param>
        /// <returns>The <c>Shape</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static Shape Circle(double radius)
        {
            return new Shape(new global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Circle(radius));
        }

        /// <summary>
        /// Creates a new <c>Shape</c> that contains a new <c>Rectangle</c> case.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>The <c>Shape</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static Shape Rectangle(double width, double height)
        {
            return new Shape(new global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Rectangle(width, height));
        }

        /// <summary>
        /// Gets the <c>Shape</c> that contains the <c>Point</c> case.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        public static Shape Point => new Shape(new global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Point());

        /// <summary>
        /// Gets all cases in the union.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
            = new global::System.Type[] { typeof(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Circle), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Rectangle), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Point) };

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
                        return this.__unmanaged.Circle;
                    case 2:
                        return this.__unmanaged.Rectangle;
                    case 3:
                        return __boxedPoint;
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
        /// Tries to get the <c>Circle</c> case.
        /// </summary>
        /// <param name="value">The case, if this instance contains it.</param>
        /// <returns><c>true</c>, if this instance contains the case, otherwise <c>false</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool TryGetValue(out global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Circle value)
        {
            if (this.__tag == 1)
            {
                value = this.__unmanaged.Circle;
                return true;
            }

            value = default!;
            return false;
        }

        /// <summary>
        /// Tries to get the <c>Rectangle</c> case.
        /// </summary>
        /// <param name="value">The case, if this instance contains it.</param>
        /// <returns><c>true</c>, if this instance contains the case, otherwise <c>false</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool TryGetValue(out global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Rectangle value)
        {
            if (this.__tag == 2)
            {
                value = this.__unmanaged.Rectangle;
                return true;
            }

            value = default!;
            return false;
        }

        /// <summary>
        /// Tries to get the <c>Point</c> case.
        /// </summary>
        /// <param name="value">The case, if this instance contains it.</param>
        /// <returns><c>true</c>, if this instance contains the case, otherwise <c>false</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool TryGetValue(out global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Point value)
        {
            value = default(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Point);
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
        public static bool operator ==(Shape left, Shape right)
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
        public static bool operator !=(Shape left, Shape right)
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
        public bool Equals(Shape other)
        {
            if (this.__tag != other.__tag)
            {
                return false;
            }

            switch (this.__tag)
            {
                case 1:
                    return global::System.Collections.Generic.EqualityComparer<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Circle>.Default.Equals(this.__unmanaged.Circle, other.__unmanaged.Circle);
                case 2:
                    return global::System.Collections.Generic.EqualityComparer<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Rectangle>.Default.Equals(this.__unmanaged.Rectangle, other.__unmanaged.Rectangle);
                default:
                    return true;
            }
        }

        /// <inheritdoc/>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override bool Equals(object? obj)
        {
            return obj is Shape other && this.Equals(other);
        }

        /// <inheritdoc/>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override int GetHashCode()
        {
            switch (this.__tag)
            {
                case 1:
                    return unchecked((1 * 397) ^ global::System.Collections.Generic.EqualityComparer<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Circle>.Default.GetHashCode(this.__unmanaged.Circle));
                case 2:
                    return unchecked((2 * 397) ^ global::System.Collections.Generic.EqualityComparer<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Rectangle>.Default.GetHashCode(this.__unmanaged.Rectangle));
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
                    return this.__unmanaged.Circle.ToString() ?? string.Empty;
                case 2:
                    return this.__unmanaged.Rectangle.ToString() ?? string.Empty;
                case 3:
                    return default(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Point).ToString() ?? string.Empty;
                default:
                    return string.Empty;
            }
        }

        [global::System.Runtime.InteropServices.StructLayoutAttribute(global::System.Runtime.InteropServices.LayoutKind.Explicit)]
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        private struct __UnmanagedCases
        {
            [global::System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Circle Circle;

            [global::System.Runtime.InteropServices.FieldOffsetAttribute(0)]
            public global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Rectangle Rectangle;
        }
    }
}
