// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClosedTypeTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tests.ClosedTypes;

using System;
using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions;
using Sundew.DiscriminatedUnions.Development.Tester.ClosedTypes;

/// <summary>
/// Tests what the generator adds to the closed types of the tester project, by using it the way a consumer would.
/// </summary>
public class ClosedTypeTests
{
    [Test]
    public void GeneratedFactory_Then_ResultShouldBeKnownAsTheClosedTypeAndBeTheCase()
    {
        var circle = Figure._Circle(2.5);
        var dot = Figure._Dot;
        var box = Figure.Box(2, 3);

        GetStaticType(circle).Should().Be(typeof(Figure));
        GetStaticType(dot).Should().Be(typeof(Figure));
        GetStaticType(box).Should().Be(typeof(Figure));
        circle.Should().Be(new Figure.Circle(2.5));
        dot.Should().BeOfType<Figure.Dot>();
        box.Should().Be(new Box(2, 3));
    }

    [Test]
    public void GeneratedFactory_When_CaseTakesNoParameters_Then_TheSameInstanceShouldBeReturned()
    {
        Figure._Dot.Should().BeSameAs(Figure._Dot);
    }

    [Test]
    public void GeneratedFactory_Then_ResultsShouldHaveTheClosedTypeAsTheirCommonType()
    {
        // Created directly, a circle and a box have no type in common, so neither of these lines would compile without naming the closed type.
        var figures = new[] { Figure._Circle(1), Figure.Box(2, 3), Figure._Dot };
        var figure = figures.Length > 2 ? Figure._Circle(1) : Figure.Box(2, 3);

        figures.Select(x => Describe(x)).Should().Equal("circle 1", "box 2x3", "dot");
        Describe(figure).Should().Be("circle 1");
    }

    [Test]
    public void GeneratedFactory_When_ClosedTypeIsGeneric_Then_ResultShouldBeKnownAsTheConstructedType()
    {
        var succeeded = Attempt<int, string>._Succeeded(1);
        var faulted = Attempt<int, string>.Faulted("failed");

        GetStaticType(succeeded).Should().Be(typeof(Attempt<int, string>));
        GetStaticType(faulted).Should().Be(typeof(Attempt<int, string>));
        succeeded.Should().Be(new Attempt<int, string>.Succeeded(1));
        faulted.Should().Be(new Faulted<int, string>("failed"));
    }

    [Test]
    public void GeneratedFactory_When_ClosedTypeDerivesFromAClosedType_Then_EachShouldReturnItsOwnType()
    {
        var animal = Animal.Dog("Rex");
        var mammal = Mammal.Dog("Rex");
        var fish = Animal.Fish;

        GetStaticType(animal).Should().Be(typeof(Animal));
        GetStaticType(mammal).Should().Be(typeof(Mammal));
        GetStaticType(fish).Should().Be(typeof(Animal));
        animal.Should().Be(new Dog("Rex"));
        mammal.Should().Be(new Dog("Rex"));
        fish.Should().BeOfType<Fish>();
    }

    [Test]
    public void Cases_Then_TheCasesASwitchHasToHandleShouldBeListed()
    {
        Figure.Cases.Should().Equal(typeof(Figure.Circle), typeof(Figure.Dot), typeof(Box));
        GetCases<Attempt<int, string>>().Should().Equal(typeof(Attempt<int, string>.Succeeded), typeof(Faulted<int, string>));
    }

    [Test]
    public void Cases_When_ClosedTypeDerivesFromAClosedType_Then_EachShouldListItsOwnCases()
    {
        Animal.Cases.Should().Equal(typeof(Dog), typeof(Cat), typeof(Fish));
        Mammal.Cases.Should().Equal(typeof(Dog), typeof(Cat));
        GetCases<Animal>().Should().Equal(typeof(Dog), typeof(Cat), typeof(Fish));
        GetCases<Mammal>().Should().Equal(typeof(Dog), typeof(Cat));
    }

    [Test]
    public void Cases_When_CaseIsPartial_Then_ItShouldListItself()
    {
        Dog.Cases.Should().Equal(typeof(Dog));
        Figure.Circle.Cases.Should().Equal(typeof(Figure.Circle));
    }

    [Test]
    public void Cases_When_CaseIsNotPartial_Then_ItShouldHaveTheListOfTheClosedTypeItDerivesFrom()
    {
        Cat.Cases.Should().Equal(typeof(Dog), typeof(Cat));

        // As for class unions, the list of a partial case is the one that is asked for by its name. A case does not implement the interface again.
        GetCases<Dog>().Should().Equal(typeof(Dog), typeof(Cat));
    }

    [Test]
    public void Segregate_Then_EachCaseShouldGetAListOfItsItems()
    {
        var result = new[] { Animal.Dog("Rex"), Animal.Fish, Animal.Cat("Tom"), Animal.Dog("Fido") }.Segregate();

        GetStaticType(result).Should().Be(typeof(AnimalSegregation));
        result.Dogs.Should().Equal(new Dog("Rex"), new Dog("Fido"));
        result.Cats.Should().Equal(new Cat("Tom"));
        result.Fishs.Should().Equal(new Fish());
    }

    [Test]
    public void Segregate_When_ClosedTypeDerivesFromAClosedType_Then_ItShouldBeSegregatedIntoItsOwnCases()
    {
        var result = new[] { Mammal.Dog("Rex"), Mammal.Cat("Tom") }.Segregate();

        GetStaticType(result).Should().Be(typeof(MammalSegregation));
        result.Dogs.Should().Equal(new Dog("Rex"));
        result.Cats.Should().Equal(new Cat("Tom"));
    }

    [Test]
    public void Segregate_When_ClosedTypeIsGeneric_Then_ListsShouldBeOfTheConstructedCases()
    {
        var result = new[] { Attempt<int, string>._Succeeded(1), Attempt<int, string>.Faulted("failed"), Attempt<int, string>._Succeeded(2) }.Segregate();

        GetStaticType(result).Should().Be(typeof(AttemptSegregation<int, string>));
        result.Succeeded.Should().Equal(new Attempt<int, string>.Succeeded(1), new Attempt<int, string>.Succeeded(2));
        result.Faulted.Should().Equal(new Faulted<int, string>("failed"));
    }

    private static Type GetStaticType<TValue>(TValue value)
    {
        // The type the compiler knows the value as, which is what a factory changes. The type of the value itself would be the same either way.
        _ = value;
        return typeof(TValue);
    }

    private static IReadOnlyList<Type> GetCases<TUnion>()
        where TUnion : IDiscriminatedUnion
    {
        return TUnion.Cases;
    }

    private static string Describe(Figure figure)
    {
        return figure switch
        {
            Figure.Circle circle => FormattableString.Invariant($"circle {circle.Radius}"),
            Figure.Dot => "dot",
            Box box => FormattableString.Invariant($"box {box.Width}x{box.Height}"),
        };
    }
}
