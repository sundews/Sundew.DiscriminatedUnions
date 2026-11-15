//HintName: Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.MammalExtensions.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes
{
    /// <summary>
    /// Contains the extension method that segregates <c>Mammal</c>.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
    public static partial class MammalExtensions
    {
        /// <summary>
        /// Segregates the items in the specified enumerable by case.
        /// </summary>
        /// <param name="mammals">The mammals.</param>
        /// <returns>A new <c>MammalSegregation</c>, which has a list for each case.</returns>
        public static global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.MammalSegregation Segregate(this global::System.Collections.Generic.IEnumerable<global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Mammal> mammals)
        {
            var dogs = new global::System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Dog>();
            var cats = new global::System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Cat>();

            foreach (var value in mammals)
            {
                switch (value)
                {
                    case global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Dog dog:
                        dogs.Add(dog);
                        break;
                    case global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Cat cat:
                        cats.Add(cat);
                        break;
                }
            }

            return new global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.MammalSegregation(dogs, cats);
        }
    }
}
