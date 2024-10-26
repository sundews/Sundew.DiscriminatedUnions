//HintName: Sundew.DiscriminatedUnions.Tester.IGenericOutParameterExtensions{T}.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Tester
{
    /// <summary>
    /// Segregation extension method for IGenericOutParameter.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.3.0.0")]
    public static partial class IGenericOutParameterExtensions
    {
        /// <summary>
        /// Segregates the items in the specified enumerable by type.
        /// </summary>
        /// <param name="iGenericOutParameters">The iGenericOutParameters.</param>
        /// <returns>A new IGenericOutParameterSegregation.</returns>
        public static IGenericOutParameterSegregation<T> Segregate<T>(this System.Collections.Generic.IEnumerable<global::Sundew.DiscriminatedUnions.Tester.IGenericOutParameter<T>> iGenericOutParameters)
            where T : notnull
        {
            var genericOutParameters = new System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Tester.GenericOutParameter<T>>();

            foreach (var value in iGenericOutParameters)
            {
                switch (value)
                {
                    case global::Sundew.DiscriminatedUnions.Tester.GenericOutParameter<T> genericOutParameter:
                        genericOutParameters.Add(genericOutParameter);
                        break;
                }
            }

            return new global::Sundew.DiscriminatedUnions.Tester.IGenericOutParameterSegregation<T>(genericOutParameters);
        }
    }
}