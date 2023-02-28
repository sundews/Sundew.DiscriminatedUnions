//HintName: Sundew.DiscriminatedUnions.Tester.DefiniteTypeExtensions.cs
namespace Sundew.DiscriminatedUnions.Tester
{
    public static class DefiniteTypeExtensions
    {
        public static DefiniteTypeSegregation Segregate(this System.Collections.Generic.IEnumerable<Sundew.DiscriminatedUnions.Tester.DefiniteType> definiteTypes)
        {
            var namedTypes = new System.Collections.Generic.List<Sundew.DiscriminatedUnions.Tester.NamedType>();
            var definiteArrayTypes = new System.Collections.Generic.List<Sundew.DiscriminatedUnions.Tester.DefiniteArrayType>();

            foreach (var value in definiteTypes)
            {
                switch (value)
                {
                    case Sundew.DiscriminatedUnions.Tester.NamedType namedType:
                        namedTypes.Add(namedType);
                        break;
                    case Sundew.DiscriminatedUnions.Tester.DefiniteArrayType definiteArrayType:
                        definiteArrayTypes.Add(definiteArrayType);
                        break;
                }
            }

            return new DefiniteTypeSegregation(namedTypes, definiteArrayTypes);
        }
    }
}