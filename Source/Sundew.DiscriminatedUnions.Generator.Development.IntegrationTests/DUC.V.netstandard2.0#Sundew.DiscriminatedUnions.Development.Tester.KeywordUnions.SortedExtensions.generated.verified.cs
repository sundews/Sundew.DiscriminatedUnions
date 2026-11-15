//HintName: Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.SortedExtensions.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions
{
    /// <summary>
    /// Contains the extension method that segregates <c>Sorted</c>.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
    public static partial class SortedExtensions
    {
        /// <summary>
        /// Segregates the items in the specified enumerable by case.
        /// </summary>
        /// <param name="sorted">The sorted.</param>
        /// <returns>A new <c>SortedSegregation</c>, which has a list for each case.</returns>
        public static global::Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.SortedSegregation Segregate(this global::System.Collections.Generic.IEnumerable<global::Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.Sorted> sorted)
        {
            var exceptions = new global::System.Collections.Generic.List<global::System.Exception>();
            var invalidOperationExceptions = new global::System.Collections.Generic.List<global::System.InvalidOperationException>();
            var int32s = new global::System.Collections.Generic.List<int?>();

            foreach (var value in sorted)
            {
                switch (value.Value)
                {
                    case global::System.InvalidOperationException invalidOperationException:
                        invalidOperationExceptions.Add(invalidOperationException);
                        break;
                    case global::System.Exception exception:
                        exceptions.Add(exception);
                        break;
                    case int int32:
                        int32s.Add(int32);
                        break;
                }
            }

            return new global::Sundew.DiscriminatedUnions.Development.Tester.KeywordUnions.SortedSegregation(exceptions, invalidOperationExceptions, int32s);
        }
    }
}
