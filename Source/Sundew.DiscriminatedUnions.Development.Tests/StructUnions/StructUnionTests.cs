// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructUnionTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tests.StructUnions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using AwesomeAssertions;
using Sundew.DiscriminatedUnions.Development.Tester.StructUnions;

// Several tests are about the default value of a union without a default case, which SDU0013 otherwise warns about creating.
#pragma warning disable SDU0013

/// <summary>
/// Tests the struct unions that the generator produces for the tester project, by using them the way a consumer would.
/// </summary>
public class StructUnionTests
{
    [Test]
    public void Default_When_UnionHasDefaultCase_Then_ItShouldBeTheDefaultCase()
    {
        Cardinality<int> testee = default;

        var result = Describe(testee);

        result.Should().Be("empty");
        testee.HasValue.Should().BeTrue();
        testee.Value.Should().Be(default(Empty<int>));
    }

    [Test]
    public void Default_When_UnionHasNoDefaultCase_Then_ItShouldHaveNoValue()
    {
        Shape testee = default;

        var result = Describe(testee);

        result.Should().Be("none");
        testee.HasValue.Should().BeFalse();
        testee.Value.Should().BeNull();
        testee.ToString().Should().BeEmpty();
    }

    [Test]
    public void ImplicitConversion_Then_UnionShouldContainTheCase()
    {
        Cardinality<int> single = new Single<int>(7);
        Cardinality<int> multiple = new Multiple<int>([1, 2, 3]);
        Shape circle = new Circle(2.5);
        Shape point = default(Point);

        Describe(single).Should().Be("single 7");
        Describe(multiple).Should().Be("multiple 3");
        Describe(circle).Should().Be("circle 2.5");
        Describe(point).Should().Be("point");
        point.HasValue.Should().BeTrue();
    }

    [Test]
    public void Factory_When_FactoryHasTheNameOfTheCase_Then_UnionShouldContainTheCase()
    {
        Describe(Cardinality<int>.Empty).Should().Be("empty");
        Describe(Cardinality<int>.Single(7)).Should().Be("single 7");
        Describe(Cardinality<int>.Multiple([1, 2])).Should().Be("multiple 2");
    }

    [Test]
    public void GeneratedFactory_Then_ResultShouldBeKnownAsTheUnionAndContainTheCase()
    {
        var circle = Shape.Circle(2.5);
        var rectangle = Shape.Rectangle(2, 3);
        var point = Shape.Point;
        var success = Outcome.Success(4);
        var found = Lookup<string>.Found("key", 5);

        GetStaticType(circle).Should().Be(typeof(Shape));
        GetStaticType(point).Should().Be(typeof(Shape));
        GetStaticType(found).Should().Be(typeof(Lookup<string>));
        Describe(circle).Should().Be("circle 2.5");
        Describe(rectangle).Should().Be("rectangle 2x3");
        Describe(point).Should().Be("point");
        Describe(success).Should().Be("success 4");
        found.Should().Be((Lookup<string>)new Found<string>("key", 5));
        Lookup<string>.Missing.Should().Be(default(Lookup<string>));
    }

    [Test]
    public void GeneratedFactory_Then_ResultsShouldHaveTheUnionAsTheirCommonType()
    {
        // Created directly, a circle and a rectangle have no type in common, so neither of these lines would compile without naming the union.
        var shapes = new[] { Shape.Circle(1), Shape.Rectangle(2, 3), Shape.Point };
        var shape = shapes.Length > 2 ? Shape.Circle(1) : Shape.Rectangle(2, 3);

        shapes.Select(x => Describe(x)).Should().Equal("circle 1", "rectangle 2x3", "point");
        Describe(shape).Should().Be("circle 1");
    }

    [Test]
    public void GeneratedFactory_When_CasesOnlyDifferByTypeArguments_Then_ArgumentShouldSelectTheCase()
    {
        var whole = Measurement.Quantity(1);
        var fraction = Measurement.Quantity(1.5);

        whole.TryGetValue(out Quantity<int> _).Should().BeTrue();
        fraction.TryGetValue(out Quantity<double> _).Should().BeTrue();
        Measurement.Unknown.Should().Be(default(Measurement));
    }

    [Test]
    public void TryGetValue_Then_OnlyTheContainedCaseShouldBeReturned()
    {
        Shape testee = new Rectangle(2, 3);

        testee.TryGetValue(out Rectangle rectangle).Should().BeTrue();
        testee.TryGetValue(out Circle circle).Should().BeFalse();
        testee.TryGetValue(out Point _).Should().BeFalse();

        rectangle.Should().Be(new Rectangle(2, 3));
        circle.Should().Be(default(Circle));
    }

    [Test]
    public void Value_Then_TheContainedCaseShouldBeReturned()
    {
        new Shape(new Rectangle(2, 3)).Value.Should().Be(new Rectangle(2, 3));
        new Shape(default(Point)).Value.Should().Be(default(Point));
        new Outcome("text").Value.Should().Be("text");
        Cardinality<int>.Single(7).Value.Should().Be(new Single<int>(7));
    }

    [Test]
    public void Value_When_CaseHasNoPayload_Then_NothingShouldBeBoxedAgain()
    {
        var first = Cardinality<int>.Empty.Value;
        var second = new Cardinality<int>(default(Empty<int>)).Value;

        first.Should().BeSameAs(second);
    }

    [Test]
    public void Equals_Then_OnlyTheContainedCaseShouldBeCompared()
    {
        Shape circle = new Circle(1);
        Shape sameCircle = new Circle(1);
        Shape otherCircle = new Circle(2);
        Shape rectangleWithTheSameBits = new Rectangle(1, 0);

        circle.Equals(sameCircle).Should().BeTrue();
        (circle == sameCircle).Should().BeTrue();
        (circle != otherCircle).Should().BeTrue();
        circle.Equals(rectangleWithTheSameBits).Should().BeFalse();
        circle.Equals((object)sameCircle).Should().BeTrue();
        circle.Equals("circle").Should().BeFalse();
        circle.GetHashCode().Should().Be(sameCircle.GetHashCode());
        default(Shape).Equals(default(Shape)).Should().BeTrue();
        default(Shape).Equals(circle).Should().BeFalse();
    }

    [Test]
    public void Equals_When_CasesAreReferences_Then_TheirValuesShouldBeCompared()
    {
        var exception = new InvalidOperationException("failed");

        new Outcome("text").Should().Be(new Outcome(new string("text".ToCharArray())));
        new Outcome("text").Should().NotBe(new Outcome("other"));
        new Outcome(exception).Should().Be(new Outcome(exception));
        new Outcome(exception).Should().NotBe(new Outcome(new InvalidOperationException("failed")));
        new Outcome(new Success(1)).Should().Be(new Outcome(new Success(1)));
    }

    [Test]
    public void ToString_Then_TheContainedCaseShouldBeDescribed()
    {
        Cardinality<int>.Single(7).ToString().Should().Be(new Single<int>(7).ToString());
        Cardinality<int>.Empty.ToString().Should().Be(default(Empty<int>).ToString());
        new Outcome("text").ToString().Should().Be("text");
    }

    [Test]
    public void Cases_Then_AllCasesShouldBeListedInTheOrderTheyAreDeclared()
    {
        Cardinality<string>.Cases.Should().Equal(typeof(Empty<string>), typeof(Single<string>), typeof(Multiple<string>));
        GetCases<Shape>().Should().Equal(typeof(Circle), typeof(Rectangle), typeof(Point));
    }

    [Test]
    public void TryGetValue_When_ReferenceCasesShareStorage_Then_OnlyTheContainedCaseShouldBeReturned()
    {
        var testee = new Outcome(new InvalidOperationException("failed"));

        testee.TryGetValue(out Exception exception).Should().BeTrue();
        testee.TryGetValue(out string text).Should().BeFalse();
        testee.TryGetValue(out Success _).Should().BeFalse();

        exception.Message.Should().Be("failed");
        text.Should().BeNull();
        Describe(testee).Should().Be("exception failed");
        Describe(new Outcome("text")).Should().Be("text text");
        Describe(new Outcome(new Success(3))).Should().Be("success 3");
    }

    [Test]
    public void Constructor_When_ReferenceCaseIsNullAndUnionHasNoDefaultCase_Then_UnionShouldHaveNoValue()
    {
        var testee = new Outcome((string)null!);

        testee.HasValue.Should().BeFalse();
        testee.Value.Should().BeNull();
        testee.Should().Be(default(Outcome));
        Describe(testee).Should().Be("none");
    }

    [Test]
    public void Constructor_When_ReferenceCaseIsNullAndUnionHasDefaultCase_Then_ArgumentNullExceptionShouldBeThrown()
    {
        Action act = () => _ = new Lookup<int>((string)null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Default_When_UnionHasDefaultCaseAndReferenceCase_Then_ItShouldBeTheDefaultCase()
    {
        Lookup<int> missing = default;
        Lookup<int> found = new Found<int>(4, 2);
        Lookup<int> redirected = "elsewhere";

        missing.TryGetValue(out Missing _).Should().BeTrue();
        found.TryGetValue(out Found<int> foundCase).Should().BeTrue();
        redirected.TryGetValue(out string text).Should().BeTrue();
        redirected.TryGetValue(out Found<int> _).Should().BeFalse();

        foundCase.Should().Be(new Found<int>(4, 2));
        text.Should().Be("elsewhere");
    }

    [Test]
    public void TryGetValue_When_CasesOnlyDifferByTypeArguments_Then_TheyShouldBeToldApart()
    {
        Measurement whole = new Quantity<int>(1);
        Measurement fraction = new Quantity<double>(1);

        whole.TryGetValue(out Quantity<int> _).Should().BeTrue();
        whole.TryGetValue(out Quantity<double> _).Should().BeFalse();
        fraction.TryGetValue(out Quantity<double> _).Should().BeTrue();
        whole.Should().NotBe(fraction);
        default(Measurement).TryGetValue(out Unknown _).Should().BeTrue();
    }

    [Test]
    public void SizeOf_Then_CasesThatCanShareStorageShouldDoSo()
    {
        // Circle (8 bytes) and Rectangle (16 bytes) are unmanaged and overlap, so Shape is the larger of them plus the tag.
        Unsafe.SizeOf<Shape>().Should().Be(24);

        // Quantity<int> (4 bytes) and Quantity<double> (8 bytes) overlap.
        Unsafe.SizeOf<Measurement>().Should().Be(16);

        // The string and the exception share a reference, next to Success (4 bytes) and the tag.
        Unsafe.SizeOf<Outcome>().Should().Be(16);

        // Every case of this generic union depends on the type parameter, so none can overlap: Single<int> (4 bytes), the reference in Multiple<int> and the tag.
        Unsafe.SizeOf<Cardinality<int>>().Should().Be(16);
    }

    [Test]
    public void SizeOf_When_UnionIsGeneric_Then_CasesThatDoNotDependOnATypeParameterShouldShareStorage()
    {
        // Instant (8 bytes) and Interval (16 bytes) overlap next to Measured<byte> (16 bytes) and the tag. With a field each it would be 48 bytes.
        Unsafe.SizeOf<Reading<byte>>().Should().Be(40);

        // The same with a reference in the case that depends on the type parameter.
        Unsafe.SizeOf<Reading<string>>().Should().Be(40);
    }

    [Test]
    public void GenericUnion_When_CasesShareStorageNextToTheUnion_Then_EachCaseShouldBeKeptAndToldApart()
    {
        Reading<string> instant = new Instant(42);
        Reading<string> interval = new Interval(1, 2);
        Reading<string> measured = new Measured<string>("m", 1.5);

        instant.TryGetValue(out Instant actualInstant).Should().BeTrue();
        interval.TryGetValue(out Interval actualInterval).Should().BeTrue();
        measured.TryGetValue(out Measured<string> actualMeasured).Should().BeTrue();
        actualInstant.Should().Be(new Instant(42));
        actualInterval.Should().Be(new Interval(1, 2));
        actualMeasured.Should().Be(new Measured<string>("m", 1.5));
        instant.TryGetValue(out Interval _).Should().BeFalse();
        interval.TryGetValue(out Instant _).Should().BeFalse();
        instant.Should().Be((Reading<string>)new Instant(42));
        instant.Should().NotBe((Reading<string>)new Instant(43));
        interval.Should().NotBe(instant);
        interval.GetHashCode().Should().Be(((Reading<string>)new Interval(1, 2)).GetHashCode());
        Reading<string>.Cases.Should().Equal(typeof(Instant), typeof(Interval), typeof(Measured<string>));
    }

    [Test]
    public void Segregate_Then_EachCaseShouldGetAListOfItsItems()
    {
        var result = new[] { Cardinality<int>.Single(1), default, Cardinality<int>.Multiple([1, 2]), Cardinality<int>.Single(2) }.Segregate();

        result.Emptys.Should().Equal(default(Empty<int>));
        result.Singles.Should().Equal(new Single<int>(1), new Single<int>(2));
        result.Multiples.Should().ContainSingle().Which.Items.Should().Equal(1, 2);
    }

    [Test]
    public void Segregate_When_CasesOnlyDifferByTypeArguments_Then_EachShouldGetAListOfItsOwn()
    {
        var result = new Measurement[] { new Quantity<int>(1), new Quantity<double>(2.5), default, new Quantity<int>(3) }.Segregate();

        result.QuantityOfInt32s.Should().Equal(new Quantity<int>(1), new Quantity<int>(3));
        result.QuantityOfDoubles.Should().Equal(new Quantity<double>(2.5));
        result.Unknowns.Should().Equal(default(Unknown));
    }

    [Test]
    public void Segregate_When_UnionHasNoValue_Then_ItShouldBeLeftOut()
    {
        var exception = new InvalidOperationException();

        var result = new Outcome[] { new Success(1), "text", default, exception, "more" }.Segregate();

        result.Successes.Should().Equal(new Success(1));
        result.Strings.Should().Equal("text", "more");
        result.Exceptions.Should().Equal(exception);
    }

    [Test]
    public void Match_Then_NothingShouldBeAllocated()
    {
        SumCardinalities(100);
        SumShapes(100);
        var allocatedBytes = GC.GetAllocatedBytesForCurrentThread();

        var result = SumCardinalities(10_000) + SumShapes(10_000);

        (GC.GetAllocatedBytesForCurrentThread() - allocatedBytes).Should().Be(0);
        result.Should().NotBe(0);
    }

    private static int SumCardinalities(int count)
    {
        var sum = 0;
        for (var i = 0; i < count; i++)
        {
            Cardinality<int> cardinality = (i % 2) == 0 ? new Single<int>(i) : default(Empty<int>);
            sum += cardinality switch
            {
                Empty<int> => 0,
                Single<int> single => single.Item,
                Multiple<int> => -1,
            };
        }

        return sum;
    }

    private static int SumShapes(int count)
    {
        var sum = 0;
        for (var i = 0; i < count; i++)
        {
            Shape shape = (i % 2) == 0 ? new Circle(i) : new Rectangle(i, i);
            sum += shape switch
            {
                Circle circle => (int)circle.Radius,
                Rectangle rectangle => (int)rectangle.Width,
                Point => 0,
                null => -1,
            };
        }

        return sum;
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

    private static string Describe(Cardinality<int> cardinality)
    {
        return cardinality switch
        {
            Empty<int> => "empty",
            Single<int> single => FormattableString.Invariant($"single {single.Item}"),
            Multiple<int> multiple => FormattableString.Invariant($"multiple {multiple.Items.Count()}"),
        };
    }

    private static string Describe(Shape shape)
    {
        return shape switch
        {
            Circle circle => FormattableString.Invariant($"circle {circle.Radius}"),
            Rectangle rectangle => FormattableString.Invariant($"rectangle {rectangle.Width}x{rectangle.Height}"),
            Point => "point",
            null => "none",
        };
    }

    private static string Describe(Outcome outcome)
    {
        return outcome switch
        {
            Success success => FormattableString.Invariant($"success {success.Code}"),
            string text => $"text {text}",
            Exception exception => $"exception {exception.Message}",
            null => "none",
        };
    }
}
