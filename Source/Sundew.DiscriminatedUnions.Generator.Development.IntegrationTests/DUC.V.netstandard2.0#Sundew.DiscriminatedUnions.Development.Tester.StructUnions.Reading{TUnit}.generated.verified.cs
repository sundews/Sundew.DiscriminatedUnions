//HintName: Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Reading{TUnit}.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.StructUnions
{
    [global::System.Runtime.CompilerServices.UnionAttribute]
    readonly partial struct Reading<TUnit> : global::System.Runtime.CompilerServices.IUnion, global::System.IEquatable<Reading<TUnit>>
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        private readonly global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.__Reading_1_UnmanagedCases __unmanaged;

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        private readonly global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Measured<TUnit> __caseMeasured;

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        private readonly byte __tag;

        /// <summary>
        /// Initializes a new instance of the <c>Reading&lt;TUnit&gt;</c> struct that contains the <c>Instant</c> case.
        /// </summary>
        /// <param name="value">The case.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public Reading(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Instant value)
        {
            this.__unmanaged = new global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.__Reading_1_UnmanagedCases { Instant = value };
            this.__tag = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <c>Reading&lt;TUnit&gt;</c> struct that contains the <c>Interval</c> case.
        /// </summary>
        /// <param name="value">The case.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public Reading(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Interval value)
        {
            this.__unmanaged = new global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.__Reading_1_UnmanagedCases { Interval = value };
            this.__tag = 2;
        }

        /// <summary>
        /// Initializes a new instance of the <c>Reading&lt;TUnit&gt;</c> struct that contains the <c>Measured&lt;TUnit&gt;</c> case.
        /// </summary>
        /// <param name="value">The case.</param>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public Reading(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Measured<TUnit> value)
        {
            this.__caseMeasured = value;
            this.__tag = 3;
        }

        /// <summary>
        /// Creates a new <c>Reading&lt;TUnit&gt;</c> that contains a new <c>Instant</c> case.
        /// </summary>
        /// <param name="ticks">The ticks.</param>
        /// <returns>The <c>Reading&lt;TUnit&gt;</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static Reading<TUnit> Instant(long ticks)
        {
            return new Reading<TUnit>(new global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Instant(ticks));
        }

        /// <summary>
        /// Creates a new <c>Reading&lt;TUnit&gt;</c> that contains a new <c>Interval</c> case.
        /// </summary>
        /// <param name="from">The from.</param>
        /// <param name="to">The to.</param>
        /// <returns>The <c>Reading&lt;TUnit&gt;</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static Reading<TUnit> Interval(long from, long to)
        {
            return new Reading<TUnit>(new global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Interval(from, to));
        }

        /// <summary>
        /// Creates a new <c>Reading&lt;TUnit&gt;</c> that contains a new <c>Measured&lt;TUnit&gt;</c> case.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="value">The value.</param>
        /// <returns>The <c>Reading&lt;TUnit&gt;</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public static Reading<TUnit> Measured(TUnit unit, double value)
        {
            return new Reading<TUnit>(new global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Measured<TUnit>(unit, value));
        }

        /// <summary>
        /// Gets all cases in the union.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
            = new global::System.Type[] { typeof(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Instant), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Interval), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Measured<TUnit>) };

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
                        return this.__unmanaged.Instant;
                    case 2:
                        return this.__unmanaged.Interval;
                    case 3:
                        return this.__caseMeasured;
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
        /// Tries to get the <c>Instant</c> case.
        /// </summary>
        /// <param name="value">The case, if this instance contains it.</param>
        /// <returns><c>true</c>, if this instance contains the case, otherwise <c>false</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool TryGetValue(out global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Instant value)
        {
            if (this.__tag == 1)
            {
                value = this.__unmanaged.Instant;
                return true;
            }

            value = default!;
            return false;
        }

        /// <summary>
        /// Tries to get the <c>Interval</c> case.
        /// </summary>
        /// <param name="value">The case, if this instance contains it.</param>
        /// <returns><c>true</c>, if this instance contains the case, otherwise <c>false</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool TryGetValue(out global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Interval value)
        {
            if (this.__tag == 2)
            {
                value = this.__unmanaged.Interval;
                return true;
            }

            value = default!;
            return false;
        }

        /// <summary>
        /// Tries to get the <c>Measured&lt;TUnit&gt;</c> case.
        /// </summary>
        /// <param name="value">The case, if this instance contains it.</param>
        /// <returns><c>true</c>, if this instance contains the case, otherwise <c>false</c>.</returns>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public bool TryGetValue(out global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Measured<TUnit> value)
        {
            value = this.__caseMeasured;
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
        public static bool operator ==(Reading<TUnit> left, Reading<TUnit> right)
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
        public static bool operator !=(Reading<TUnit> left, Reading<TUnit> right)
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
        public bool Equals(Reading<TUnit> other)
        {
            if (this.__tag != other.__tag)
            {
                return false;
            }

            switch (this.__tag)
            {
                case 1:
                    return global::System.Collections.Generic.EqualityComparer<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Instant>.Default.Equals(this.__unmanaged.Instant, other.__unmanaged.Instant);
                case 2:
                    return global::System.Collections.Generic.EqualityComparer<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Interval>.Default.Equals(this.__unmanaged.Interval, other.__unmanaged.Interval);
                case 3:
                    return global::System.Collections.Generic.EqualityComparer<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Measured<TUnit>>.Default.Equals(this.__caseMeasured, other.__caseMeasured);
                default:
                    return true;
            }
        }

        /// <inheritdoc/>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override bool Equals(object? obj)
        {
            return obj is Reading<TUnit> other && this.Equals(other);
        }

        /// <inheritdoc/>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
        public override int GetHashCode()
        {
            switch (this.__tag)
            {
                case 1:
                    return unchecked((1 * 397) ^ global::System.Collections.Generic.EqualityComparer<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Instant>.Default.GetHashCode(this.__unmanaged.Instant));
                case 2:
                    return unchecked((2 * 397) ^ global::System.Collections.Generic.EqualityComparer<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Interval>.Default.GetHashCode(this.__unmanaged.Interval));
                case 3:
                    return unchecked((3 * 397) ^ global::System.Collections.Generic.EqualityComparer<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Measured<TUnit>>.Default.GetHashCode(this.__caseMeasured));
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
                    return this.__unmanaged.Instant.ToString() ?? string.Empty;
                case 2:
                    return this.__unmanaged.Interval.ToString() ?? string.Empty;
                case 3:
                    return this.__caseMeasured.ToString() ?? string.Empty;
                default:
                    return string.Empty;
            }
        }
    }

    /// <summary>
    /// Stores the cases of <c>Reading&lt;TUnit&gt;</c> that contain no references and do not depend on a type parameter, so that they can overlap.
    /// It is a type of its own, because the runtime does not allow a generic type to lay out its fields explicitly.
    /// </summary>
    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
    [global::System.Runtime.InteropServices.StructLayoutAttribute(global::System.Runtime.InteropServices.LayoutKind.Explicit)]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
    internal struct __Reading_1_UnmanagedCases
    {
        [global::System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Instant Instant;

        [global::System.Runtime.InteropServices.FieldOffsetAttribute(0)]
        public global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Interval Interval;
    }
}
