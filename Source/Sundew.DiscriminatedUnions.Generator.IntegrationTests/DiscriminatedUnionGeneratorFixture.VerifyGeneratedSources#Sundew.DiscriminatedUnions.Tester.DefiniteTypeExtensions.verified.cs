//HintName: Sundew.DiscriminatedUnions.Tester.DefiniteTypeExtensions.cs
namespace Sundew.DiscriminatedUnions.Tester
{
    public static class DefiniteTypeExtensions
    {
        public static DefiniteTypeSegregation Segregate(this System.Collections.Generic.IEnumerable<global::Sundew.DiscriminatedUnions.Tester.DefiniteType> definiteTypes)
        {
            var namedTypes = new System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Tester.NamedType>();
            var definiteArrayTypes = new System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Tester.DefiniteArrayType>();

            foreach (var value in definiteTypes)
            {
                switch (value)
                {
                    case global::Sundew.DiscriminatedUnions.Tester.NamedType namedType:
                        namedTypes.Add(namedType);
                        break;
                    case global::Sundew.DiscriminatedUnions.Tester.DefiniteArrayType definiteArrayType:
                        definiteArrayTypes.Add(definiteArrayType);
                        break;
                }
            }

            return new DefiniteTypeSegregation(namedTypes, definiteArrayTypes);
        }
    }
}