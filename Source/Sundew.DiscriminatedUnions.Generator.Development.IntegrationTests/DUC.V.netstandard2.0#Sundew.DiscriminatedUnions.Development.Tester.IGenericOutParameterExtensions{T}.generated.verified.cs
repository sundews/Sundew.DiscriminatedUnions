//HintName: Sundew.DiscriminatedUnions.Development.Tester.IGenericOutParameterExtensions{T}.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester
{
    /// <summary>
    /// Segregation extension method for IGenericOutParameter.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.4.0.0")]
    public static partial class IGenericOutParameterExtensions
    {
        /// <summary>
        /// Segregates the items in the specified enumerable by type.
        /// </summary>
        /// <param name="iGenericOutParameters">The iGenericOutParameters.</param>
        /// <returns>A new IGenericOutParameterSegregation.</returns>
        public static IGenericOutParameterSegregation<T> Segregate<T>(this System.Collections.Generic.IEnumerable<global::Sundew.DiscriminatedUnions.Development.Tester.IGenericOutParameter<T>> iGenericOutParameters)
            where T : notnull
        {
            var genericOutParameters = new System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Development.Tester.GenericOutParameter<T>>();
            var specialGenericOutParameters = new System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Development.Tester.SpecialGenericOutParameter<T>>();

            foreach (var value in iGenericOutParameters)
            {
                switch (value)
                {
                    case global::Sundew.DiscriminatedUnions.Development.Tester.GenericOutParameter<T> genericOutParameter:
                        genericOutParameters.Add(genericOutParameter);
                        break;
                    case global::Sundew.DiscriminatedUnions.Development.Tester.SpecialGenericOutParameter<T> specialGenericOutParameter:
                        specialGenericOutParameters.Add(specialGenericOutParameter);
                        break;
                }
            }

            return new global::Sundew.DiscriminatedUnions.Development.Tester.IGenericOutParameterSegregation<T>(genericOutParameters, specialGenericOutParameters);
        }
    }
}