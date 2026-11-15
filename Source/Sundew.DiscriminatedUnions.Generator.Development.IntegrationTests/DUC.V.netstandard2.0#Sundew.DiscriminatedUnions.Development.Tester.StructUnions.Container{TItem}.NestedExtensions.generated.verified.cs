//HintName: Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Container{TItem}.NestedExtensions.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.StructUnions
{
    /// <summary>
    /// Contains the extension method that segregates <c>Nested</c>.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
    static partial class NestedExtensions
    {
        /// <summary>
        /// Segregates the items in the specified enumerable by case.
        /// </summary>
        /// <param name="nested">The nested.</param>
        /// <returns>A new <c>NestedSegregation&lt;TItem&gt;</c>, which has a list for each case.</returns>
        internal static global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.NestedSegregation<TItem> Segregate<TItem>(this global::System.Collections.Generic.IEnumerable<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Container<TItem>.Nested> nested)
        {
            var int32s = new global::System.Collections.Generic.List<int>();
            var int64s = new global::System.Collections.Generic.List<long>();
            var wrappers = new global::System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Container<TItem>.Wrapper>();

            foreach (var value in nested)
            {
                if (value.TryGetValue(out int int32))
                {
                    int32s.Add(int32);
                }
                else if (value.TryGetValue(out long int64))
                {
                    int64s.Add(int64);
                }
                else if (value.TryGetValue(out global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.Container<TItem>.Wrapper wrapper))
                {
                    wrappers.Add(wrapper);
                }
            }

            return new global::Sundew.DiscriminatedUnions.Development.Tester.StructUnions.NestedSegregation<TItem>(int32s, int64s, wrappers);
        }
    }
}
