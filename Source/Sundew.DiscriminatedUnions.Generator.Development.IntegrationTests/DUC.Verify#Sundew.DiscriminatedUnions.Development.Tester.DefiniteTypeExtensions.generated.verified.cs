//HintName: Sundew.DiscriminatedUnions.Development.Tester.DefiniteTypeExtensions.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester
{
    /// <summary>
    /// Segregation extension method for DefiniteType.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "5.3.0.0")]
    public static partial class DefiniteTypeExtensions
    {
        /// <summary>
        /// Segregates the items in the specified enumerable by type.
        /// </summary>
        /// <param name="definiteTypes">The definiteTypes.</param>
        /// <returns>A new DefiniteTypeSegregation.</returns>
        public static DefiniteTypeSegregation Segregate(this System.Collections.Generic.IEnumerable<global::Sundew.DiscriminatedUnions.Development.Tester.DefiniteType> definiteTypes)
        {
            var definiteArrayTypes = new System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Development.Tester.DefiniteArrayType>();
            var namedTypes = new System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Development.Tester.NamedType>();

            foreach (var value in definiteTypes)
            {
                switch (value)
                {
                    case global::Sundew.DiscriminatedUnions.Development.Tester.DefiniteArrayType definiteArrayType:
                        definiteArrayTypes.Add(definiteArrayType);
                        break;
                    case global::Sundew.DiscriminatedUnions.Development.Tester.NamedType namedType:
                        namedTypes.Add(namedType);
                        break;
                }
            }

            return new global::Sundew.DiscriminatedUnions.Development.Tester.DefiniteTypeSegregation(definiteArrayTypes, namedTypes);
        }
    }
}