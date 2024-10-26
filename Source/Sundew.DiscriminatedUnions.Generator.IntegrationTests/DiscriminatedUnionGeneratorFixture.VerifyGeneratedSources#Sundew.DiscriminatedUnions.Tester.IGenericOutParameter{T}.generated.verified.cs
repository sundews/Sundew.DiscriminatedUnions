//HintName: Sundew.DiscriminatedUnions.Tester.IGenericOutParameter{T}.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Tester
{
#pragma warning disable SA1601
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.3.0.0")]
    public partial interface IGenericOutParameter<out T>
        where T : notnull
#pragma warning restore SA1601
    {
        /// <summary>
        /// Factory method for the GenericOutParameter case.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A new GenericOutParameter.</returns>
        [Sundew.DiscriminatedUnions.CaseType(typeof(global::Sundew.DiscriminatedUnions.Tester.GenericOutParameter<>))]
        [global::System.Diagnostics.DebuggerNonUserCode]
        public static global::Sundew.DiscriminatedUnions.Tester.IGenericOutParameter<T> GenericOutParameter(T value)
            => new global::Sundew.DiscriminatedUnions.Tester.GenericOutParameter<T>(value);
    }
}
