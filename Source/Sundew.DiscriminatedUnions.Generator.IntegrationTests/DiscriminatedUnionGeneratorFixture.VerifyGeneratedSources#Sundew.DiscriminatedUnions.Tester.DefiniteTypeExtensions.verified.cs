//HintName: Sundew.DiscriminatedUnions.Tester.DefiniteTypeExtensions.cs
namespace Sundew.DiscriminatedUnions.Tester
{
    /// <summary>
    /// Segregation extension method for DefiniteType.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "3.0.0.0")]
    public static partial class DefiniteTypeExtensions
    {
        /// <summary>
        /// Segregates the items in the specified enumerable by type.
        /// </summary>
        /// <param name="definiteTypes">The definiteTypes.</param>
        /// <returns>A new DefiniteTypeSegregation.</returns>
        public static DefiniteTypeSegregation Segregate(this System.Collections.Generic.IEnumerable<global::Sundew.DiscriminatedUnions.Tester.DefiniteType> definiteTypes)
        {
            var definiteArrayTypes = new System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Tester.DefiniteArrayType>();
            var namedTypes = new System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Tester.NamedType>();

            foreach (var value in definiteTypes)
            {
                switch (value)
                {
                    case global::Sundew.DiscriminatedUnions.Tester.DefiniteArrayType definiteArrayType:
                        definiteArrayTypes.Add(definiteArrayType);
                        break;
                    case global::Sundew.DiscriminatedUnions.Tester.NamedType namedType:
                        namedTypes.Add(namedType);
                        break;
                }
            }

            return new global::Sundew.DiscriminatedUnions.Tester.DefiniteTypeSegregation(definiteArrayTypes, namedTypes);
        }
    }
}