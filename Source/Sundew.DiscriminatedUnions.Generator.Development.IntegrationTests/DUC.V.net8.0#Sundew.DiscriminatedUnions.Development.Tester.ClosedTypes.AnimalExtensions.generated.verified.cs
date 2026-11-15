//HintName: Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.AnimalExtensions.generated.cs
#nullable enable

namespace Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes
{
    /// <summary>
    /// Contains the extension method that segregates <c>Animal</c>.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Sundew.DiscriminateUnions.Generator", "7.0.0.0")]
    public static partial class AnimalExtensions
    {
        /// <summary>
        /// Segregates the items in the specified enumerable by case.
        /// </summary>
        /// <param name="animals">The animals.</param>
        /// <returns>A new <c>AnimalSegregation</c>, which has a list for each case.</returns>
        public static global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.AnimalSegregation Segregate(this global::System.Collections.Generic.IEnumerable<global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Animal> animals)
        {
            var dogs = new global::System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Dog>();
            var cats = new global::System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Cat>();
            var fishs = new global::System.Collections.Generic.List<global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Fish>();

            foreach (var value in animals)
            {
                switch (value)
                {
                    case global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Dog dog:
                        dogs.Add(dog);
                        break;
                    case global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Cat cat:
                        cats.Add(cat);
                        break;
                    case global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.Fish fish:
                        fishs.Add(fish);
                        break;
                }
            }

            return new global::Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes.AnimalSegregation(dogs, cats, fishs);
        }
    }
}
