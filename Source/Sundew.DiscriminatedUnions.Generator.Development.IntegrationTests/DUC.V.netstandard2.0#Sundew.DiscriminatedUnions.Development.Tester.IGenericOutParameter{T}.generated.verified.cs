//HintName: Sundew.DiscriminatedUnions.Development.Tester.IGenericOutParameter{T}.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester
{
#pragma warning disable SA1601
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "6.0.0.0")]
    public partial interface IGenericOutParameter<out T>
        where T : notnull
#pragma warning restore SA1601
    {
        /// <summary>
        /// Factory method for the GenericOutParameter case.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A new GenericOutParameter.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.GenericOutParameter<>))]
        [global::System.Diagnostics.DebuggerNonUserCode]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.IGenericOutParameter<T> GenericOutParameter(T value)
            => new global::Sundew.DiscriminatedUnions.Development.Tester.GenericOutParameter<T>(value);

        /// <summary>
        /// Factory method for the SpecialGenericOutParameter case.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A new SpecialGenericOutParameter.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Development.Tester.SpecialGenericOutParameter<>))]
        [global::System.Diagnostics.DebuggerNonUserCode]
        public static global::Sundew.DiscriminatedUnions.Development.Tester.IGenericOutParameter<T> SpecialGenericOutParameter(T value)
            => new global::Sundew.DiscriminatedUnions.Development.Tester.SpecialGenericOutParameter<T>(value);

        /// <summary>
        /// Gets all cases in the union.
        /// </summary>
        /// <returns>A readonly list of types.</returns>
        public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }
            = new global::System.Type[] { typeof(global::Sundew.DiscriminatedUnions.Development.Tester.GenericOutParameter<T>), typeof(global::Sundew.DiscriminatedUnions.Development.Tester.SpecialGenericOutParameter<T>) };
    }
}
