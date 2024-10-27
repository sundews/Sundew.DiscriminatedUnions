//HintName: Sundew.DiscriminatedUnions.Tester.IGenericOutParameterSegregation{T}.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Tester
{
    /// <summary>
    /// Contains individual lists of the different cases of the discriminated union IGenericOutParameter.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.3.0.0")]
    public sealed partial class IGenericOutParameterSegregation<T>
        where T : notnull
    {
        internal IGenericOutParameterSegregation(System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.GenericOutParameter<T>> genericOutParameters, System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.SpecialGenericOutParameter<T>> specialGenericOutParameters)
        {
            this.GenericOutParameters = genericOutParameters;
            this.SpecialGenericOutParameters = specialGenericOutParameters;
        }

        /// <summary>
        /// Gets the GenericOutParameters.
        /// </summary>
        /// <returns>The GenericOutParameters.</returns>
        public System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.GenericOutParameter<T>> GenericOutParameters { get; }

        /// <summary>
        /// Gets the SpecialGenericOutParameters.
        /// </summary>
        /// <returns>The SpecialGenericOutParameters.</returns>
        public System.Collections.Generic.IReadOnlyList<global::Sundew.DiscriminatedUnions.Tester.SpecialGenericOutParameter<T>> SpecialGenericOutParameters { get; }
    }
}