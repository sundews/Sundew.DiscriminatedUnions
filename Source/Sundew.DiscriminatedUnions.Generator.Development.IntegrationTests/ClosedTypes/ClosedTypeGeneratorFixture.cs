// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClosedTypeGeneratorFixture.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Development.IntegrationTests.ClosedTypes;

using System;
using System.Linq;
using System.Threading.Tasks;
using AwesomeAssertions;
using Sundew.DiscriminatedUnions.Generator.Development.IntegrationTests.Unions;

/// <summary>
/// Tests what is generated for partial closed types: the factories, which create a case and return it as the closed type, and the list of cases.
/// </summary>
public class ClosedTypeGeneratorFixture
{
    private const string CasesProperty = "public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }";
    private const string CaseListInterface = "global::Sundew.DiscriminatedUnions.IDiscriminatedUnion";
    private const string Shape = """
        public closed partial record Shape
        {
            public sealed record Circle(double Radius) : Shape;

            public sealed record Point : Shape;
        }

        public sealed record Square(double Side = 1.5) : Shape;

        """;

    [Test]
    public void Run_Then_EachCaseShouldGetAFactoryThatReturnsTheClosedType()
    {
        var result = GeneratorRunner.Run(Shape);

        result.GeneratorDiagnostics.Should().BeEmpty();
        result.CompilationDiagnostics.Should().BeEmpty();
        result.GeneratedSources.Select(x => x.HintName).Should().Equal("Shape.generated.cs");
        var source = result.GeneratedSources[0].Source;
        GetFactories(source).Should().Equal(
            "public static Shape _Circle(double radius)",
            "public static Shape _Point { get; }",
            "public static Shape Square(double side = 1.5)");
        source.Should().Contain("partial record Shape")
            .And.Contain("return new global::Shape.Circle(radius);")
            .And.Contain("= new global::Shape.Point();")
            .And.Contain("return new global::Square(side);");
    }

    [Test]
    public void Run_When_ClosedTypeIsAClass_Then_GeneratedPartShouldBeAClass()
    {
        var source = Generate("""
            public closed partial class Node;
            public sealed class Leaf(int value) : Node { public int Value => value; }
            """);

        GetFactories(source).Should().Equal("public static Node Leaf(int value)");
        source.Should().Contain("partial class Node");
    }

    [Test]
    [Arguments("public closed record Shape; public sealed record Circle(double Radius) : Shape;")]
    [Arguments("[DiscriminatedUnion(true)] public closed partial record Shape; public sealed record Circle(double Radius) : Shape;")]
    [Arguments("[DiscriminatedUnion(disableGenerator: true)] public closed partial record Shape; public sealed record Circle(double Radius) : Shape;")]
    [Arguments("public closed partial record Shape;")]
    public void Run_When_ClosedTypeIsNotPartialOrGeneratorIsDisabledOrThereAreNoCases_Then_NothingShouldBeGenerated(string source)
    {
        var result = GeneratorRunner.Run(source);

        result.GeneratorDiagnostics.Should().BeEmpty();
        result.CompilationDiagnostics.Should().BeEmpty();
        result.GeneratedSources.Should().BeEmpty();
    }

    [Test]
    [Arguments("[DiscriminatedUnion]")]
    [Arguments("[DiscriminatedUnion(false)]")]
    [Arguments("[DiscriminatedUnion(GeneratorFeatures.None)]")]
    public void Run_When_ClosedTypeHasTheAttribute_Then_ItsMembersShouldBeGeneratedAllTheSame(string attribute)
    {
        var result = GeneratorRunner.Run(attribute + " public closed partial record Shape { public sealed partial record Circle(double Radius) : Shape; } public sealed partial record Square(double Side) : Shape;");

        result.GeneratorDiagnostics.Should().BeEmpty();
        result.CompilationDiagnostics.Should().BeEmpty();
        result.GeneratedSources.Select(x => x.HintName).Should().Equal("Shape.generated.cs", "Shape.Circle.generated.cs", "Square.generated.cs");
        GetFactories(result.GeneratedSources[0].Source).Should().Equal("public static Shape _Circle(double radius)", "public static Shape Square(double side)");
    }

    [Test]
    public void Run_When_ClosedTypeDeclaresAMemberWithTheNameOfAFactory_Then_ThatFactoryShouldNotBeGenerated()
    {
        var source = Generate("""
            public closed partial record Shape
            {
                public sealed record Circle(double Radius) : Shape;

                public static Shape _Circle(double radius) => new Circle(radius * 2);

                public static Shape Square(int side) => new Square(side);
            }

            public sealed record Square(double Side) : Shape;
            public sealed record Triangle(double Side) : Shape;
            """);

        GetFactories(source).Should().Equal("public static Shape Triangle(double side)");
    }

    [Test]
    public void Run_When_ClosedTypeDerivesFromAClosedType_Then_BothShouldGetFactoriesForItsCases()
    {
        var result = GeneratorRunner.Run("""
            public closed partial record Outer;
            public closed partial record Inner : Outer;
            public sealed record Leaf(int Value) : Inner;
            public sealed record Direct : Outer;
            public abstract record Open : Outer;
            public sealed record BelowOpen : Open;
            """);

        result.GeneratorDiagnostics.Should().BeEmpty();
        result.CompilationDiagnostics.Should().BeEmpty();
        result.GeneratedSources.Select(x => x.HintName).Should().Equal("Outer.generated.cs", "Inner.generated.cs");
        GetFactories(result.GeneratedSources[0].Source).Should().Equal("public static Outer Leaf(int value)", "public static Outer Direct { get; }");
        GetFactories(result.GeneratedSources[1].Source).Should().Equal("public static Inner Leaf(int value)");
    }

    [Test]
    public void Run_When_CaseIsNestedInAClosedTypeBetweenItAndTheClosedType_Then_FactoryShouldHaveTheSameNameInBoth()
    {
        var result = GeneratorRunner.Run("""
            public closed partial record Outer
            {
                public closed partial record Inner : Outer
                {
                    public sealed record Leaf(int Value) : Inner;
                }
            }
            """);

        result.GeneratorDiagnostics.Should().BeEmpty();
        result.CompilationDiagnostics.Should().BeEmpty();
        GetFactories(result.GeneratedSources.Single(x => x.HintName == "Outer.generated.cs").Source).Should().Equal("public static Outer _Leaf(int value)");
        GetFactories(result.GeneratedSources.Single(x => x.HintName == "Outer.Inner.generated.cs").Source).Should().Equal("public static Inner _Leaf(int value)");
    }

    [Test]
    public void Run_When_ClosedTypeIsGeneric_Then_FactoriesShouldUseItsTypeParameters()
    {
        var source = Generate("""
            public closed partial record Result<TValue, TError>
            {
                public sealed record Success(TValue Value) : Result<TValue, TError>;
            }

            public sealed record Failure<TValue, TError>(TError Error) : Result<TValue, TError>;
            public sealed record Swapped<TError, TValue>(TError Error) : Result<TValue, TError>;
            public sealed record IntOnly(int Value) : Result<int, string>;
            """);

        GetFactories(source).Should().Equal(
            "public static Result<TValue, TError> _Success(TValue value)",
            "public static Result<TValue, TError> Failure(TError error)",
            "public static Result<TValue, TError> Swapped(TError error)");
        source.Should().Contain("return new global::Result<TValue, TError>.Success(value);")
            .And.Contain("return new global::Swapped<TError, TValue>(error);");
    }

    [Test]
    public void Run_When_CaseIsInternal_Then_FactoryShouldBeInternal()
    {
        var source = Generate("""
            public closed partial record Shape
            {
                private sealed record Hidden : Shape;
            }

            internal sealed record Square(double Side) : Shape;
            """);

        GetFactories(source).Should().Equal("internal static Shape Square(double side)");
    }

    [Test]
    public async Task Run_When_ClosedTypeHasTheAttribute_Then_AnalyzerShouldNotTreatItAsAClassUnion()
    {
        // The compiler checks a closed type, so the rules for class unions do not apply to it. An unnested case of a class union has to have a factory, for example.
        var result = await GeneratorRunner.AnalyzeAsync("""
            [DiscriminatedUnion(disableGenerator: true)]
            public closed partial record Shape
            {
                public sealed record Circle(double Radius) : Shape;
            }

            public record Square(double Side) : Shape;

            public static class Shapes
            {
                public static double Measure(Shape shape) => shape switch
                {
                    Shape.Circle circle => circle.Radius,
                    Square square => square.Side,
                };
            }
            """);

        result.Should().BeEmpty();
    }

    [Test]
    public void Run_Then_CasesShouldBeListedInTheOrderTheCompilerListsThem()
    {
        var source = Generate(Shape);

        source.Should().Contain("partial record Shape : " + CaseListInterface)
            .And.Contain(CasesProperty)
            .And.Contain("= new global::System.Type[] { typeof(global::Shape.Circle), typeof(global::Shape.Point), typeof(global::Square) };");
    }

    [Test]
    [Arguments("net11.0", true)]
    [Arguments("net8.0", true)]
    [Arguments("net6.0", false)]
    [Arguments("netstandard2.0", false)]
    public void Run_Then_CaseListInterfaceShouldOnlyBeImplementedWhereItExists(string targetFramework, bool expectedResult)
    {
        var source = GeneratorRunner.Run(Shape, targetFramework).GeneratedSources.Single().Source;

        source.Contains(CaseListInterface).Should().Be(expectedResult);
        source.Should().Contain(CasesProperty);
    }

    [Test]
    public void Run_When_ClosedTypeDeclaresCases_Then_OnlyTheFactoriesShouldBeGenerated()
    {
        var source = Generate("public closed partial record Shape { public static int Cases => 1; } public sealed record Circle(double Radius) : Shape;");

        GetFactories(source).Should().Equal("public static Shape Circle(double radius)");
        source.Should().NotContain(CaseListInterface).And.NotContain(CasesProperty);
    }

    [Test]
    public void Run_When_ClosedTypeDerivesFromAClosedType_Then_EachShouldListTheCasesASwitchHasToHandle()
    {
        var result = GeneratorRunner.Run("""
            public closed partial record Outer;
            public closed partial record Inner : Outer;
            public sealed record Leaf(int Value) : Inner;
            public sealed record Direct : Outer;
            public abstract record Open : Outer;
            public sealed record BelowOpen : Open;
            """);

        // The list of the inner type hides the one it inherits, which must not be a warning.
        result.GeneratorDiagnostics.Should().BeEmpty();
        result.CompilationDiagnostics.Should().BeEmpty();
        result.GeneratedSources[0].Source.Should().Contain("{ typeof(global::Leaf), typeof(global::Direct), typeof(global::Open) };");
        result.GeneratedSources[1].Source.Should().Contain("partial record Inner : " + CaseListInterface).And.Contain("{ typeof(global::Leaf) };");
    }

    [Test]
    public void Run_When_ClosedTypeIsGeneric_Then_CasesShouldBeTheOnesOfEveryConstruction()
    {
        var source = Generate("""
            public closed partial record Result<TValue, TError>
            {
                public sealed record Success(TValue Value) : Result<TValue, TError>;
            }

            public sealed record Failure<TValue, TError>(TError Error) : Result<TValue, TError>;
            public sealed record IntOnly(int Value) : Result<int, string>;
            """);

        source.Should().Contain("{ typeof(global::Result<TValue, TError>.Success), typeof(global::Failure<TValue, TError>) };");
    }

    [Test]
    public void Run_When_CaseIsHiddenWithinTheClosedType_Then_ItShouldStillBeListed()
    {
        var source = Generate("public closed partial record Shape { private sealed record Hidden : Shape; }");

        GetFactories(source).Should().BeEmpty();
        source.Should().Contain("{ typeof(global::Shape.Hidden) };");
    }

    [Test]
    public void Run_When_CaseIsPartial_Then_ItShouldGetAListOfCasesThatListsItself()
    {
        var result = GeneratorRunner.Run("""
            public closed partial record Shape
            {
                public sealed partial record Circle(double Radius) : Shape;
            }

            public sealed partial record Square(double Side) : Shape;
            public sealed record Triangle(double Side) : Shape;
            """);

        // The list of a case hides the one it inherits from the closed type, which must not be a warning.
        result.GeneratorDiagnostics.Should().BeEmpty();
        result.CompilationDiagnostics.Should().BeEmpty();
        result.GeneratedSources.Select(x => x.HintName).Should().Equal("Shape.generated.cs", "Shape.Circle.generated.cs", "Square.generated.cs");
        result.GeneratedSources[1].Source.Should().Contain("partial record Shape")
            .And.Contain("partial record Circle")
            .And.Contain(CasesProperty)
            .And.Contain("= new global::System.Type[] { typeof(global::Shape.Circle) };")
            .And.NotContain(CaseListInterface);
        result.GeneratedSources[2].Source.Should().Contain("partial record Square").And.Contain("{ typeof(global::Square) };");
    }

    [Test]
    public void Run_When_CaseIsGeneric_Then_ItsListOfCasesShouldUseItsTypeParameters()
    {
        var result = GeneratorRunner.Run("public closed partial record Result<TValue>; public sealed partial record Success<TItem>(TItem Value) : Result<TItem>;");

        result.CompilationDiagnostics.Should().BeEmpty();
        result.GeneratedSources.Select(x => x.HintName).Should().Equal("Result{TValue}.generated.cs", "Success{TItem}.generated.cs");
        result.GeneratedSources[1].Source.Should().Contain("partial record Success<TItem>").And.Contain("{ typeof(global::Success<TItem>) };");
    }

    [Test]
    public void Run_When_CaseIsAPartialClass_Then_GeneratedPartShouldBeAClass()
    {
        var result = GeneratorRunner.Run("public closed partial class Node; public sealed partial class Leaf : Node; public abstract partial class Branch : Node;");

        result.CompilationDiagnostics.Should().BeEmpty();
        result.GeneratedSources.Select(x => x.HintName).Should().Equal("Node.generated.cs", "Leaf.generated.cs", "Branch.generated.cs");
        result.GeneratedSources[1].Source.Should().Contain("partial class Leaf").And.Contain("{ typeof(global::Leaf) };");
        result.GeneratedSources[2].Source.Should().Contain("partial class Branch").And.Contain("{ typeof(global::Branch) };");
    }

    [Test]
    public void Run_When_CaseDerivesFromAClosedTypeThatIsNotPartial_Then_APartialClosedTypeAboveItShouldBeEnough()
    {
        var result = GeneratorRunner.Run("public closed partial record Outer; public closed record Inner : Outer; public sealed partial record Leaf : Inner;");

        result.CompilationDiagnostics.Should().BeEmpty();
        result.GeneratedSources.Select(x => x.HintName).Should().Equal("Outer.generated.cs", "Leaf.generated.cs");
    }

    [Test]
    [Arguments("public closed partial record Shape; public sealed record Circle(double Radius) : Shape;")]
    [Arguments("public closed record Shape; public sealed partial record Circle(double Radius) : Shape;")]
    [Arguments("[DiscriminatedUnion(disableGenerator: true)] public closed partial record Shape; public sealed partial record Circle(double Radius) : Shape;")]
    [Arguments("public closed partial record Shape; public sealed partial record Circle(double Radius) : Shape { public new static int Cases => 1; }")]
    [Arguments("public closed partial record Shape; public static class Holder { public sealed partial record Circle(double Radius) : Shape; }")]
    [Arguments("public closed partial record Shape; public abstract record Open : Shape; public sealed partial record Circle(double Radius) : Open;")]
    public void Run_When_TypeIsNotAPartialCaseOfAPartialClosedType_Then_ItShouldNotGetAListOfCases(string source)
    {
        var result = GeneratorRunner.Run(source);

        result.GeneratorDiagnostics.Should().BeEmpty();
        result.CompilationDiagnostics.Should().BeEmpty();
        result.GeneratedSources.Select(x => x.HintName).Should().NotContain("Circle.generated.cs").And.NotContain("Holder.Circle.generated.cs");
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
            .Where(x => (x.StartsWith("public static ", StringComparison.Ordinal) || x.StartsWith("internal static ", StringComparison.Ordinal)) && x != CasesProperty)
            .ToArray();
    }
}