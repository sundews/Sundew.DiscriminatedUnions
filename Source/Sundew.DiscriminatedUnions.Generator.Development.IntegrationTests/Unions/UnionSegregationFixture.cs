// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionSegregationFixture.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Development.IntegrationTests.Unions;

using System.Linq;
using System.Threading.Tasks;
using AwesomeAssertions;

/// <summary>
/// Tests what is generated for a union that asks to be segregated, whether it is a struct union, a union declaration or a closed type:
/// a type with a list per case and an extension method that distributes the unions in an enumerable to those lists.
/// </summary>
public class UnionSegregationFixture
{
    private const string Segregate = "[DiscriminatedUnion(GeneratorFeatures.Segregate)] ";
    private const string Cases = """
        public readonly record struct Circle(double Radius);
        public readonly record struct Square(double Side);

        """;

    private const string StructUnion = Cases + Segregate + "public readonly partial struct Shape : ICase<Circle>, ICase<Square>;";
    private const string UnionDeclaration = Cases + Segregate + "public partial union Shape(Circle, Square);";
    private const string ClosedType = Segregate + "public closed partial record Shape; public sealed record Circle(double Radius) : Shape; public sealed record Square(double Side) : Shape;";

    [Test]
    [Arguments(StructUnion, "if (value.TryGetValue(out global::Circle circle))", "else if (value.TryGetValue(out global::Square square))")]
    [Arguments(UnionDeclaration, "switch (value.Value)", "case global::Square square:")]
    [Arguments(ClosedType, "switch (value)", "case global::Square square:")]
    public void Run_When_UnionAsksToBeSegregated_Then_TheTypesThatDoSoShouldBeGeneratedAndCompileWithoutWarnings(string source, string expectedFirstMatch, string expectedSecondMatch)
    {
        var result = GeneratorRunner.Run(source);

        result.GeneratorDiagnostics.Should().BeEmpty();
        result.CompilationDiagnostics.Should().BeEmpty();
        result.GeneratedSources.Select(x => x.HintName).Should().Equal("Shape.generated.cs", "ShapeSegregation.generated.cs", "ShapeExtensions.generated.cs");
        result.GeneratedSources[1].Source.Should().Contain("public sealed partial class ShapeSegregation")
            .And.Contain("internal ShapeSegregation(global::System.Collections.Generic.IReadOnlyList<global::Circle> circles, global::System.Collections.Generic.IReadOnlyList<global::Square> squares)")
            .And.Contain("public global::System.Collections.Generic.IReadOnlyList<global::Circle> Circles { get; }")
            .And.Contain("public global::System.Collections.Generic.IReadOnlyList<global::Square> Squares { get; }");
        result.GeneratedSources[2].Source.Should().Contain("public static partial class ShapeExtensions")
            .And.Contain("public static global::ShapeSegregation Segregate(this global::System.Collections.Generic.IEnumerable<global::Shape> shapes)")
            .And.Contain("var circles = new global::System.Collections.Generic.List<global::Circle>();")
            .And.Contain(expectedFirstMatch)
            .And.Contain(expectedSecondMatch)
            .And.Contain("squares.Add(square);")
            .And.Contain("return new global::ShapeSegregation(circles, squares);");
    }

    [Test]
    [Arguments(Cases + Segregate + "public union Shape(Circle, Square);")]
    [Arguments(Segregate + "public closed record Shape; public sealed record Circle(double Radius) : Shape; public sealed record Square(double Side) : Shape;")]
    public void Run_When_UnionIsNotPartial_Then_OnlyTheTypesThatSegregateItShouldBeGenerated(string source)
    {
        var result = GeneratorRunner.Run(source);

        result.GeneratorDiagnostics.Should().BeEmpty();
        result.CompilationDiagnostics.Should().BeEmpty();
        result.GeneratedSources.Select(x => x.HintName).Should().Equal("ShapeSegregation.generated.cs", "ShapeExtensions.generated.cs");
    }

    [Test]
    [Arguments("[DiscriminatedUnion] public readonly partial struct Shape : ICase<Circle>, ICase<Square>;")]
    [Arguments("[DiscriminatedUnion(GeneratorFeatures.None)] public partial union Shape(Circle, Square);")]
    [Arguments("public partial union Shape(Circle, Square);")]
    public void Run_When_UnionDoesNotAskToBeSegregated_Then_OnlyItsMembersShouldBeGenerated(string union)
    {
        var result = GeneratorRunner.Run(Cases + union);

        result.CompilationDiagnostics.Should().BeEmpty();
        result.GeneratedSources.Select(x => x.HintName).Should().Equal("Shape.generated.cs");
    }

    [Test]
    public void Run_When_UnionIsInANamespace_Then_TheTypesThatSegregateItShouldBeDeclaredThere()
    {
        var result = GeneratorRunner.Run("namespace Shapes.Flat { " + UnionDeclaration + " }");

        result.CompilationDiagnostics.Should().BeEmpty();
        result.GeneratedSources.Select(x => x.HintName).Should().Equal("Shapes.Flat.Shape.generated.cs", "Shapes.Flat.ShapeSegregation.generated.cs", "Shapes.Flat.ShapeExtensions.generated.cs");
        result.GeneratedSources[1].Source.Should().Contain("namespace Shapes.Flat");
        result.GeneratedSources[2].Source.Should().Contain("namespace Shapes.Flat")
            .And.Contain("public static global::Shapes.Flat.ShapeSegregation Segregate(this global::System.Collections.Generic.IEnumerable<global::Shapes.Flat.Shape> shapes)");
    }

    [Test]
    public void Run_When_UnionIsGeneric_Then_TheTypesThatSegregateItShouldRepeatItsTypeParametersAndTheirConstraints()
    {
        var result = GeneratorRunner.Run(Segregate + """
            public closed partial record Result<TValue, TError>
                where TValue : notnull
                where TError : class, IComparable<TError>, new()
            {
                public sealed record Success(TValue Value) : Result<TValue, TError>;
            }

            public sealed record Failure<TValue, TError>(TError Error) : Result<TValue, TError>
                where TValue : notnull
                where TError : class, IComparable<TError>, new();
            """);

        result.CompilationDiagnostics.Should().BeEmpty();
        result.GeneratedSources.Select(x => x.HintName).Should().Equal("Result{TValue,TError}.generated.cs", "ResultSegregation{TValue,TError}.generated.cs", "ResultExtensions{TValue,TError}.generated.cs");
        GetLines(result.GeneratedSources[1].Source).Should().ContainInOrder(
            "public sealed partial class ResultSegregation<TValue, TError>",
            "where TValue : notnull",
            "where TError : class, global::System.IComparable<TError>, new()",
            "public global::System.Collections.Generic.IReadOnlyList<global::Result<TValue, TError>.Success> Successes { get; }",
            "public global::System.Collections.Generic.IReadOnlyList<global::Failure<TValue, TError>> Failures { get; }");
        GetLines(result.GeneratedSources[2].Source).Should().ContainInOrder(
            "public static partial class ResultExtensions",
            "public static global::ResultSegregation<TValue, TError> Segregate<TValue, TError>(this global::System.Collections.Generic.IEnumerable<global::Result<TValue, TError>> results)",
            "where TValue : notnull",
            "where TError : class, global::System.IComparable<TError>, new()",
            "case global::Result<TValue, TError>.Success success:",
            "return new global::ResultSegregation<TValue, TError>(successes, failures);");
    }

    [Test]
    [Arguments("unmanaged", "where T : unmanaged")]
    [Arguments("struct", "where T : struct")]
    [Arguments("class", "where T : class")]
    [Arguments("class?", "where T : class?")]
    [Arguments("notnull", "where T : notnull")]
    [Arguments("new()", "where T : new()")]
    [Arguments("class, IDisposable, new()", "where T : class, global::System.IDisposable, new()")]
    [Arguments("IComparable<T>, IEquatable<T>", "where T : global::System.IComparable<T>, global::System.IEquatable<T>")]
    public void Run_When_TypeParameterIsConstrained_Then_TheTypesThatSegregateTheUnionShouldHaveTheSameConstraint(string constraint, string expectedResult)
    {
        var result = GeneratorRunner.Run($"public readonly record struct Wrapper<T>(int Value) where T : {constraint}; {Segregate}public readonly partial struct Box<T> : ICase<Wrapper<T>>, ICase<int> where T : {constraint};");

        result.CompilationDiagnostics.Should().BeEmpty();
        GetLines(result.GeneratedSources[1].Source).Should().Contain(expectedResult);
        GetLines(result.GeneratedSources[2].Source).Should().Contain(expectedResult);
    }

    [Test]
    public void Run_When_UnionIsNestedInAGenericType_Then_TheTypesThatSegregateItShouldGetTheTypeParametersOfThatType()
    {
        var result = GeneratorRunner.Run(Cases + "public partial class Outer<TItem> where TItem : notnull { " + Segregate + "public partial union Inner<TOther>(Circle, TItem[], List<TOther>); }");

        result.CompilationDiagnostics.Should().BeEmpty();
        result.GeneratedSources.Select(x => x.HintName).Should().Equal("Outer{TItem}.Inner{TOther}.generated.cs", "Outer{TItem}.InnerSegregation{TOther}.generated.cs", "Outer{TItem}.InnerExtensions{TOther}.generated.cs");
        GetLines(result.GeneratedSources[1].Source).Should().ContainInOrder(
            "public sealed partial class InnerSegregation<TItem, TOther>",
            "where TItem : notnull",
            "public global::System.Collections.Generic.IReadOnlyList<TItem[]> TItemArrays { get; }",
            "public global::System.Collections.Generic.IReadOnlyList<global::System.Collections.Generic.List<TOther>> Lists { get; }");
        GetLines(result.GeneratedSources[2].Source).Should().Contain(
            "public static global::InnerSegregation<TItem, TOther> Segregate<TItem, TOther>(this global::System.Collections.Generic.IEnumerable<global::Outer<TItem>.Inner<TOther>> inners)");
    }

    [Test]
    public void Run_When_CasesAreNotDeclaredTypes_Then_TheirListsShouldBeNamedAfterTheTypeTheyAre()
    {
        var result = GeneratorRunner.Run(Segregate + "public union Mixed(string, int?, int[], (int Number, string Name), List<string?>);");

        result.CompilationDiagnostics.Should().BeEmpty();
        GetLines(result.GeneratedSources[0].Source).Should().ContainInOrder(
            "public global::System.Collections.Generic.IReadOnlyList<string> Strings { get; }",
            "public global::System.Collections.Generic.IReadOnlyList<int?> Int32s { get; }",
            "public global::System.Collections.Generic.IReadOnlyList<int[]> Int32Arrays { get; }",
            "public global::System.Collections.Generic.IReadOnlyList<(int Number, string Name)> ValueTuples { get; }",
            "public global::System.Collections.Generic.IReadOnlyList<global::System.Collections.Generic.List<string?>> Lists { get; }");

        // In a pattern a nullable value type is the type it wraps and a tuple is the type it is, because both have a syntax that means something else there.
        GetLines(result.GeneratedSources[1].Source).Should().ContainInOrder(
            "case string @string:",
            "case int int32:",
            "case int[] int32Array:",
            "case global::System.ValueTuple<int, string> valueTuple:",
            "case global::System.Collections.Generic.List<string?> list:");
    }

    [Test]
    [Arguments("public readonly record struct Quantity<TValue>(TValue Value);", "Quantity<int>", "Quantity<double>", "QuantityOfInt32s", "QuantityOfDoubles")]
    [Arguments("namespace First { public readonly record struct Item(int Value); } namespace Second { public readonly record struct Item(int Value); }", "First.Item", "Second.Item", "Item1s", "Item2s")]
    public void Run_When_CasesShareAName_Then_TheirListsShouldBeToldApart(string cases, string firstCase, string secondCase, string expectedFirstName, string expectedSecondName)
    {
        var result = GeneratorRunner.Run($"{cases} {Segregate}public readonly partial struct Amount : ICase<{firstCase}>, ICase<{secondCase}>;");

        result.CompilationDiagnostics.Should().BeEmpty();
        result.GeneratedSources[1].Source.Should().Contain($"> {expectedFirstName} {{ get; }}").And.Contain($"> {expectedSecondName} {{ get; }}");
    }

    [Test]
    public void Run_When_TypeTellsTheCaseAndCasesAreRelated_Then_TheMostSpecificTypeShouldBeMatchedFirst()
    {
        var result = GeneratorRunner.Run(Segregate + "public union Problem<T>(T, object, Exception, IEnumerable<object>, InvalidOperationException, List<string>);");

        // A type parameter can be any type, so it follows the types that are known, except for the ones it converts to itself.
        result.CompilationDiagnostics.Should().BeEmpty();
        GetLines(result.GeneratedSources[1].Source).Where(x => x.StartsWith("case ", System.StringComparison.Ordinal)).Should().Equal(
            "case global::System.InvalidOperationException invalidOperationException:",
            "case global::System.Exception exception:",
            "case global::System.Collections.Generic.List<string> list:",
            "case global::System.Collections.Generic.IEnumerable<object> iEnumerable:",
            "case T t:",
            "case object @object:");
        GetLines(result.GeneratedSources[1].Source).Should().Contain("return new global::ProblemSegregation<T>(ts, objects, exceptions, iEnumerables, invalidOperationExceptions, lists);");
    }

    [Test]
    public void Run_When_StructUnionHasRelatedAndNullableCases_Then_TheMostSpecificCaseShouldBeAskedFirstAndANullableCaseAsTheTypeItWraps()
    {
        var result = GeneratorRunner.Run(Segregate + "public readonly partial struct Problem : ICase<Exception>, ICase<InvalidOperationException>, ICase<int?>;");

        // A struct union hands out a value as every case it is, so the case that says most about the value has to be asked first.
        result.GeneratorDiagnostics.Should().BeEmpty();
        result.CompilationDiagnostics.Should().BeEmpty();
        GetLines(result.GeneratedSources.Single(x => x.HintName.Contains("Extensions")).Source).Where(x => x.Contains("TryGetValue")).Should().Equal(
            "if (value.TryGetValue(out global::System.InvalidOperationException invalidOperationException))",
            "else if (value.TryGetValue(out global::System.Exception exception))",
            "else if (value.TryGetValue(out int int32))");
        result.GeneratedSources.Single(x => x.HintName.Contains("Segregation")).Source.Should().Contain("IReadOnlyList<int?> Int32s { get; }");
    }

    [Test]
    public void Segregate_When_StructUnionHasRelatedAndNullableCases_Then_EachItemShouldGoToTheMostSpecificCaseItIs()
    {
        var result = GeneratorRunner.Load(Segregate + """
            public readonly partial struct Problem : ICase<Exception>, ICase<InvalidOperationException>, ICase<int?>;

            public static class Program
            {
                public static string Run()
                {
                    Problem[] problems = [new Problem((Exception)new InvalidOperationException()), new ArgumentException(), 4, new Problem((int?)null), new InvalidOperationException()];
                    var segregation = problems.Segregate();
                    return $"{segregation.Exceptions.Count} {segregation.InvalidOperationExceptions.Count} {segregation.Int32s.Count}";
                }
            }
            """);

        result.Diagnostics.Should().BeEmpty();
        result.Assembly!.GetType("Program")!.GetMethod("Run")!.Invoke(null, null).Should().Be("1 2 1");
    }

    [Test]
    public void Run_When_NamesWouldCollide_Then_TheyShouldBeToldApartAndCompile()
    {
        var result = GeneratorRunner.Run("""
            public sealed record Value;
            public sealed record Values;
            public sealed record @default;
            public sealed record Equal;
            [DiscriminatedUnion(GeneratorFeatures.Segregate)]
            public partial union Fixed(Value, Values, @default, Equal);
            """);

        result.GeneratorDiagnostics.Should().BeEmpty();
        result.CompilationDiagnostics.Should().BeEmpty();
        GetLines(result.GeneratedSources[2].Source).Should().ContainInOrder(
            "public static global::FixedSegregation Segregate(this global::System.Collections.Generic.IEnumerable<global::Fixed> @fixed)",
            "foreach (var value in @fixed)",
            "case global::Value value_:",
            "case global::@default @default:");
        GetLines(result.GeneratedSources[1].Source).Should().Contain("public global::System.Collections.Generic.IReadOnlyList<global::Equal> Equals_ { get; }");
    }

    [Test]
    [Arguments("internal readonly record struct Circle(double Radius); public readonly record struct Square(double Side); " + Segregate + "internal readonly partial struct Shape : ICase<Circle>, ICase<Square>;")]
    [Arguments("public sealed record Circle(double Radius); internal sealed record Hidden; " + Segregate + "public partial union Shape(Circle, List<Hidden[]>);")]
    [Arguments(Segregate + "public closed partial record Shape; public sealed record Circle(double Radius) : Shape; internal sealed record Square(double Side) : Shape;")]
    [Arguments("public sealed record Circle(double Radius); public sealed record Square(double Side); internal static partial class Outer { " + Segregate + "public partial union Shape(Circle, Square); }")]
    public void Run_When_UnionOrOneOfItsCasesIsInternal_Then_TheTypesThatSegregateItShouldBeInternal(string source)
    {
        var result = GeneratorRunner.Run(source);

        result.GeneratorDiagnostics.Should().BeEmpty();
        result.CompilationDiagnostics.Should().BeEmpty();
        var segregation = GetLines(result.GeneratedSources.Single(x => x.HintName.EndsWith("ShapeSegregation.generated.cs", System.StringComparison.Ordinal)).Source);
        var extensions = GetLines(result.GeneratedSources.Single(x => x.HintName.EndsWith("ShapeExtensions.generated.cs", System.StringComparison.Ordinal)).Source);
        segregation.Should().Contain("internal sealed partial class ShapeSegregation");
        extensions.Should().Contain("static partial class ShapeExtensions");
        extensions.Where(x => x.Contains(" Segregate(")).Should().ContainSingle().Which.Should().StartWith("internal static ");
    }

    [Test]
    [Arguments(Segregate + "public closed partial record Shape { private sealed record Hidden : Shape; } public sealed record Circle(double Radius) : Shape;", "Shape.Hidden")]
    [Arguments("public sealed record Circle(double Radius); public sealed record Square(double Side); public static partial class Outer { " + Segregate + "private partial union Shape(Circle, Square); }", "Outer.Shape")]
    [Arguments("public sealed record Circle(double Radius); public partial class Outer { private sealed record Hidden; " + Segregate + "public partial union Shape(Circle, List<Hidden>); }", "List<Outer.Hidden>")]
    [Arguments("public readonly record struct Circle(double Radius); public partial class Outer { protected readonly record struct Hidden; " + Segregate + "public readonly partial struct Shape : ICase<Circle>, ICase<Hidden>; }", "Outer.Hidden")]
    public void Run_When_UnionOrOneOfItsCasesIsHiddenWithinAType_Then_ItShouldNotBeSegregatedAndTheReasonShouldBeReported(string source, string expectedTypeName)
    {
        // The types that segregate a union are declared in its namespace, from where they could not refer to what is hidden within a type.
        var result = GeneratorRunner.Run(source);

        result.GeneratedSources.Select(x => x.HintName).Should().ContainSingle().Which.Should().EndWith("Shape.generated.cs");
        var diagnostic = result.GeneratorDiagnostics.Should().ContainSingle().Which;
        diagnostic.Id.Should().Be("GDU0007");
        diagnostic.Severity.Should().Be(Microsoft.CodeAnalysis.DiagnosticSeverity.Warning);
        diagnostic.GetMessage(System.Globalization.CultureInfo.InvariantCulture).Should().Be($"The union Shape cannot be segregated, because the types that segregate it could not refer to {expectedTypeName} from the namespace of the union");
        diagnostic.Location.GetLineSpan().StartLinePosition.Character.Should().Be(source.IndexOf(" Shape", System.StringComparison.Ordinal) + 1);
    }

    [Test]
    public void Run_When_UnionHasATypeParameterNamedLikeOneOfTheTypeItIsNestedIn_Then_ItShouldNotBeSegregatedAndTheReasonShouldBeReported()
    {
        // The types that segregate the union would need both type parameters, which they could not have under one name.
        var result = GeneratorRunner.Run(Cases + "public partial class Outer<T> { " + Segregate + "public partial union Inner<T>(Circle, T[]); }");

        result.GeneratedSources.Select(x => x.HintName).Should().Equal("Outer{T}.Inner{T}.generated.cs");
        var diagnostic = result.GeneratorDiagnostics.Should().ContainSingle().Which;
        diagnostic.Id.Should().Be("GDU0007");
        diagnostic.GetMessage(System.Globalization.CultureInfo.InvariantCulture).Should().Contain("could not refer to Outer<T>.Inner<T> from the namespace of the union");
    }

    [Test]
    public void Run_When_ClosedTypeDerivesFromAClosedType_Then_EachShouldBeSegregatedIntoItsOwnCases()
    {
        var result = GeneratorRunner.Run(Segregate + "public closed record Outer; " + Segregate + "public closed record Inner : Outer; public sealed record Leaf : Inner; public sealed record Direct : Outer; public abstract record Open : Outer;");

        result.CompilationDiagnostics.Should().BeEmpty();
        result.GeneratedSources.Select(x => x.HintName).Should().Equal("OuterSegregation.generated.cs", "OuterExtensions.generated.cs", "InnerSegregation.generated.cs", "InnerExtensions.generated.cs");
        GetLines(result.GeneratedSources[1].Source).Should().Contain("return new global::OuterSegregation(leafs, directs, opens);");
        GetLines(result.GeneratedSources[3].Source).Should().Contain("return new global::InnerSegregation(leafs);");
    }

    [Test]
    [Arguments(StructUnion)]
    [Arguments(UnionDeclaration)]
    [Arguments(ClosedType)]
    public async Task Run_When_UnionHasTheAttribute_Then_CompilerShouldStillWarnAboutASwitchThatDoesNotHandleAllCases(string source)
    {
        // The compiler checks these unions. For class unions the analyzer does, which is why it silences the compiler for them, and only for them.
        var result = await GeneratorRunner.GetWarningIdsAsync(source + " public static class Shapes { public static double Measure(Shape shape) => shape switch { Circle circle => circle.Radius }; }");

        result.Should().Equal("CS8509");
    }

    private static string[] GetLines(string source)
    {
        return source.ReplaceLineEndings().Split(System.Environment.NewLine).Select(x => x.Trim()).ToArray();
    }
}
