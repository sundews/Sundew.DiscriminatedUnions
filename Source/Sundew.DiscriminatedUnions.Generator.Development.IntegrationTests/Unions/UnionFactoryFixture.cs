// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionFactoryFixture.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Development.IntegrationTests.Unions;

using System;
using System.Linq;
using AwesomeAssertions;

/// <summary>
/// Tests the factories that are generated for struct unions and union declarations: the members that create a union from a new instance of a case,
/// so that the result is known as the union and not as the case.
/// </summary>
public class UnionFactoryFixture
{
    private const string Cases = """
        public readonly record struct Empty;
        public readonly record struct Circle(double Radius);
        public sealed record Rectangle(double Width, double Height);

        """;

    private const string DeclaredMembers = """
            public static int Cases => 2;
            public override bool Equals(object? obj) => false;
            public override int GetHashCode() => 0;
            public override string ToString() => string.Empty;
        """;

    [Test]
    [Arguments("public readonly partial struct Shape : IDefaultCase<Empty>, ICase<Circle>, ICase<Rectangle>;", "public static Shape Empty => new Shape(new global::Empty());")]
    [Arguments("public partial union Shape(Empty, Circle, Rectangle);", "public static Shape Empty { get; }")]
    public void Run_Then_EachCaseShouldGetAFactoryThatReturnsTheUnion(string union, string expectedEmptyFactory)
    {
        var source = Generate(Cases + union);

        GetFactories(source).Should().Equal(
            expectedEmptyFactory,
            "public static Shape Circle(double radius)",
            "public static Shape Rectangle(double width, double height)");
        source.Should().Contain("new Shape(new global::Empty());")
            .And.Contain("return new Shape(new global::Circle(radius));")
            .And.Contain("return new Shape(new global::Rectangle(width, height));");
    }

    [Test]
    [Arguments("public readonly record struct Marker; public readonly partial struct Selection : ICase<Marker>, ICase<int>;", "public static Selection Marker => new Selection(new global::Marker());")]
    [Arguments("public sealed class Marker; public readonly partial struct Selection : ICase<Marker>, ICase<int>;", "public static Selection Marker { get; }")]
    [Arguments("public readonly record struct Marker; public partial union Selection(Marker, int);", "public static Selection Marker { get; }")]
    public void Run_When_CaseTakesNoParameters_Then_FactoryShouldOnlyKeepItsUnionWhenCreatingItAllocates(string source, string expectedResult)
    {
        var result = Generate(source);

        GetFactories(result).Should().Equal(expectedResult);
    }

    [Test]
    [Arguments("public readonly partial struct Shape : ICase<Circle>, ICase<Rectangle> { public static Shape Circle(double radius) => new Shape(new global::Circle(radius * 2)); }")]
    [Arguments("public partial union Shape(Circle, Rectangle) { public static Shape Circle(double radius) => new Shape(new global::Circle(radius * 2)); }")]
    public void Run_When_UnionDeclaresAMemberWithTheNameOfACase_Then_ThatFactoryShouldNotBeGenerated(string union)
    {
        var source = Generate(Cases + union);

        GetFactories(source).Should().Equal("public static Shape Rectangle(double width, double height)");
    }

    [Test]
    [Arguments("public readonly partial struct Outcome : ICase<Circle>, ICase<string>, ICase<Exception>, ICase<List<int>>, ICase<int[]>;")]
    [Arguments("public partial union Outcome(Circle, string, Exception, List<int>, int[]);")]
    public void Run_When_CaseIsNotDeclaredInTheAssemblyOfTheUnion_Then_ItShouldNotGetAFactory(string union)
    {
        var source = Generate(Cases + union);

        GetFactories(source).Should().Equal("public static Outcome Circle(double radius)");
    }

    [Test]
    public void Run_When_UnionIsGeneric_Then_FactoryShouldUseItsTypeParameters()
    {
        var source = Generate("""
            public readonly record struct Found<TKey>(TKey Key, int Index);
            public readonly partial struct Lookup<TKey> : ICase<Found<TKey>>, ICase<string>;
            """);

        GetFactories(source).Should().Equal("public static Lookup<TKey> Found(TKey key, int index)");
        source.Should().Contain("return new Lookup<TKey>(new global::Found<TKey>(key, index));");
    }

    [Test]
    public void Run_When_CasesShareANameAndDifferInParameters_Then_TheirFactoriesShouldOverload()
    {
        var source = Generate("""
            public readonly record struct Quantity<TValue>(TValue Value);
            public readonly partial struct Measurement : ICase<Quantity<int>>, ICase<Quantity<double>>;
            """);

        GetFactories(source).Should().Equal("public static Measurement Quantity(int value)", "public static Measurement Quantity(double value)");
    }

    [Test]
    [Arguments("namespace First { public readonly record struct Item(int Value); } namespace Second { public readonly record struct Item(int Value); }")]
    [Arguments("namespace First { public readonly record struct Item; } namespace Second { public readonly record struct Item(int Value); }")]
    public void Run_When_CasesShareANameAndCannotOverload_Then_TheyShouldNotGetFactories(string cases)
    {
        var source = Generate(cases + " public readonly partial struct Selection : ICase<First.Item>, ICase<Second.Item>, ICase<string>;");

        GetFactories(source).Should().BeEmpty();
    }

    [Test]
    public void Run_When_ConstructorHasDefaultValues_Then_FactoryShouldHaveTheSameDefaultValues()
    {
        var source = Generate("""
            public enum Kind { First, Second }
            public readonly record struct Options(string Name = "x y", char Symbol = 'c', double Factor = 1.5, float Ratio = 0.5f, decimal Amount = 2.5m, long Size = 5, bool IsOn = true, Kind Kind = Kind.Second, Kind? Other = Kind.First, int? Count = null, string? Note = null);
            public readonly partial struct Setting : ICase<Options>, ICase<int>;
            """);

        GetFactories(source).Should().Equal(
            """public static Setting Options(string name = "x y", char symbol = 'c', double factor = 1.5, float ratio = 0.5F, decimal amount = 2.5M, long size = 5, bool isOn = true, global::Kind kind = global::Kind.Second, global::Kind? other = global::Kind.First, int? count = default, string? note = default)""");
    }

    [Test]
    public void Run_When_ConstructorTakesAVariableNumberOfArguments_Then_FactoryShouldToo()
    {
        var source = Generate("""
            public sealed class Group { public Group(string name, params int[] items) { } }
            public readonly partial struct Selection : ICase<Group>, ICase<int>;
            """);

        GetFactories(source).Should().Equal("public static Selection Group(string name, params int[] items)");
    }

    [Test]
    public void Run_When_ParameterIsNamedLikeAKeyword_Then_ItShouldBeEscaped()
    {
        var source = Generate("""
            public readonly record struct Marker(int @class);
            public readonly partial struct Selection : ICase<Marker>, ICase<string>;
            """);

        GetFactories(source).Should().Equal("public static Selection Marker(int @class)");
        source.Should().Contain("return new Selection(new global::Marker(@class));");
    }

    [Test]
    public void Run_When_CaseHasSeveralConstructors_Then_FactoryShouldMirrorTheAccessibleOneWithTheMostParameters()
    {
        var source = Generate("""
            public sealed class Range
            {
                public Range(int to) { }
                public Range(int from, int to) { }
                private Range(int from, int to, int step) { }
            }
            public readonly partial struct Selection : ICase<Range>, ICase<int>;
            """);

        GetFactories(source).Should().Equal("public static Selection Range(int from, int to)");
    }

    [Test]
    public void Run_When_ConstructorIsInternal_Then_FactoryShouldBeInternal()
    {
        var source = Generate("""
            public sealed class Token { internal Token(string text) { } }
            public readonly partial struct Selection : ICase<Token>, ICase<int>;
            """);

        GetFactories(source).Should().Equal("internal static Selection Token(string text)");
    }

    [Test]
    [Arguments("public abstract class Node { }")]
    [Arguments("public sealed class Node { private Node() { } }")]
    [Arguments("public sealed class Node { public required string Name { get; init; } }")]
    [Arguments("public sealed class Node { [Obsolete] public Node() { } }")]
    [Arguments("public sealed class Node { public Node(ref int value) { } }")]
    public void Run_When_CaseCannotBeCreatedByAFactory_Then_ItShouldNotGetOne(string @case)
    {
        var source = Generate(@case + " public readonly partial struct Selection : ICase<Node>, ICase<int>;");

        GetFactories(source).Should().BeEmpty();
    }

    [Test]
    public void Run_When_ConstructorSetsTheRequiredMembers_Then_CaseShouldGetAFactory()
    {
        var source = Generate("""
            public sealed class Node
            {
                [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
                public Node(string name) => this.Name = name;

                public required string Name { get; init; }
            }
            public readonly partial struct Selection : ICase<Node>, ICase<int>;
            """);

        GetFactories(source).Should().Equal("public static Selection Node(string name)");
    }

    [Test]
    public void Run_When_UnionDeclarationDeclaresEverythingButTheFactories_Then_OnlyTheFactoriesShouldBeGenerated()
    {
        var source = Generate(Cases + "public partial union Shape(Circle, Rectangle) {" + DeclaredMembers + "}");

        GetFactories(source).Should().Equal("public static Shape Circle(double radius)", "public static Shape Rectangle(double width, double height)");
        source.Should().NotContain("Cases").And.NotContain("Equals").And.NotContain("ToString");
    }

    [Test]
    public void Run_When_UnionDeclarationDeclaresEverythingIncludingTheFactories_Then_NothingShouldBeGenerated()
    {
        var result = GeneratorRunner.Run(Cases + "public partial union Shape(Circle, Rectangle) {" + DeclaredMembers + """
                public static Shape Circle(double radius) => new Shape(new global::Circle(radius));
                public static Shape Rectangle(double width, double height) => new Shape(new global::Rectangle(width, height));
            }
            """);

        result.GeneratorDiagnostics.Should().BeEmpty();
        result.CompilationDiagnostics.Should().BeEmpty();
        result.GeneratedSources.Should().BeEmpty();
    }

    private static string Generate(string source)
    {
        var result = GeneratorRunner.Run(source);
        result.GeneratorDiagnostics.Should().BeEmpty();
        result.CompilationDiagnostics.Should().BeEmpty();
        return result.GeneratedSources.Single().Source;
    }

    private static string[] GetFactories(string source)
    {
        return source.ReplaceLineEndings()
            .Split(Environment.NewLine)
            .Select(x => x.Trim())
            .Where(x => (x.StartsWith("public static ", StringComparison.Ordinal) || x.StartsWith("internal static ", StringComparison.Ordinal))
                        && !x.Contains(" operator ", StringComparison.Ordinal)
                        && !x.Contains(" Cases ", StringComparison.Ordinal))
            .ToArray();
    }
}