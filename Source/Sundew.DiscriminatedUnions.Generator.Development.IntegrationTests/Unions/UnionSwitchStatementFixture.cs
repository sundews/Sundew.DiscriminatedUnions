// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionSwitchStatementFixture.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Development.IntegrationTests.Unions;

using System.Linq;
using System.Threading.Tasks;
using AwesomeAssertions;

/// <summary>
/// Tests that a switch statement over a struct union, a union declaration or a closed type has to handle every case.
/// The compiler checks that for switch expressions only, so for switch statements the analyzer reports SDU0001, as it does for class unions.
/// </summary>
public class UnionSwitchStatementFixture
{
    private const string Cases = "public readonly record struct Circle(double Radius); public readonly record struct Square(double Side); public sealed record Polygon(int Sides); ";
    private const string StructUnion = Cases + "public readonly partial struct Shape : ICase<Circle>, ICase<Square>, ICase<Polygon>;";
    private const string UnionDeclaration = Cases + "public union Shape(Circle, Square, Polygon);";
    private const string ClosedType = "public closed record Shape; public sealed record Circle(double Radius) : Shape; public sealed record Square(double Side) : Shape; public sealed record Polygon(int Sides) : Shape;";

    [Test]
    [Arguments(StructUnion)]
    [Arguments(UnionDeclaration)]
    [Arguments(ClosedType)]
    public async Task Analyze_When_SwitchStatementLeavesOutCases_Then_TheyShouldBeReportedOnTheStatement(string union)
    {
        var diagnostics = await GeneratorRunner.AnalyzeAsync(GetSwitch(union, "Shape", "case Circle: break;"));

        var diagnostic = diagnostics.Should().ContainSingle().Which;
        diagnostic.Id.Should().Be("SDU0001");
        diagnostic.GetMessage().Should().Be("'Square', 'Polygon' cases in 'Shape' are not handled.");
        diagnostic.Location.SourceTree!.GetText().ToString(diagnostic.Location.SourceSpan).Should().StartWith("switch (value)").And.EndWith("}");
    }

    [Test]
    [Arguments(StructUnion, "case Circle: break; case Square: break; case Polygon: break;")]
    [Arguments(UnionDeclaration, "case Circle circle: break; case Square square: break; case Polygon polygon: break;")]
    [Arguments(ClosedType, "case Circle: break; case Square _: break; case Polygon { }: break;")]
    [Arguments(StructUnion, "case Circle or Square: break; case Polygon: break;")]
    [Arguments(UnionDeclaration, "case Circle: break; case not null: break;")]
    [Arguments(ClosedType, "case Circle(var radius): break; case Square { Side: var side }: break; case Polygon: break;")]
    [Arguments(StructUnion, "case Circle: break; default: break;")]
    [Arguments(UnionDeclaration, "case Circle: break; case var other: break;")]
    [Arguments(StructUnion, "case Circle: break; case { }: break;")]
    [Arguments(StructUnion, "case object: break;")]
    [Arguments(UnionDeclaration, "case Circle: break; case System.ValueType: break; case Polygon and not null: break;")]
    [Arguments(ClosedType, "case Circle: break; case Shape: break;")]
    public async Task Analyze_When_SwitchStatementHandlesEveryCase_Then_NothingShouldBeReported(string union, string sections)
    {
        var diagnostics = await GeneratorRunner.AnalyzeAsync(GetSwitch(union, "Shape", sections));

        diagnostics.Should().BeEmpty();
    }

    [Test]
    [Arguments(StructUnion, "case Circle circle when circle.Radius > 1: break; case Square: break; case Polygon: break;")]
    [Arguments(UnionDeclaration, "case Circle { Radius: > 1 }: break; case Square: break; case Polygon: break;")]
    [Arguments(ClosedType, "case Circle(> 1): break; case Square: break; case Polygon: break;")]
    [Arguments(StructUnion, "case Circle and { Radius: 1 }: break; case Square: break; case Polygon: break;")]
    public async Task Analyze_When_ACaseIsOnlyHandledForSomeOfItsValues_Then_ItShouldBeReported(string union, string sections)
    {
        var diagnostics = await GeneratorRunner.AnalyzeAsync(GetSwitch(union, "Shape", sections));

        diagnostics.Select(x => x.GetMessage()).Should().Equal("'Circle' case in 'Shape' is not handled.");
    }

    [Test]
    [Arguments(StructUnion)]
    [Arguments(UnionDeclaration)]
    [Arguments(ClosedType)]
    public async Task Analyze_When_SwitchIsAnExpression_Then_ItShouldBeLeftToTheCompiler(string union)
    {
        var diagnostics = await GeneratorRunner.AnalyzeAsync(union + " public static class Program { public static int Run(Shape value) => value switch { Circle => 1 }; }");

        diagnostics.Should().BeEmpty();
    }

    [Test]
    [Arguments(StructUnion, "Shape?")]
    [Arguments(UnionDeclaration, "Shape?")]
    [Arguments(ClosedType, "Shape?")]
    [Arguments("public sealed record Name(string Text); public union Shape(Name?, int);", "Shape")]
    [Arguments("public sealed record Name(string Text); public union Shape(Name, int?);", "Shape")]
    [Arguments("public sealed record Name(string Text); public readonly partial struct Shape : ICase<Name>, ICase<int?>;", "Shape")]
    public async Task Analyze_When_ValueCanBeNullAndNullIsNotHandled_Then_NullShouldBeReported(string union, string valueType)
    {
        var sections = union.Contains("Name") ? "case Name: break; case int: break;" : "case Circle: break; case Square: break; case Polygon: break;";

        var diagnostics = await GeneratorRunner.AnalyzeAsync(GetSwitch(union, valueType, sections));
        var diagnosticsWithNull = await GeneratorRunner.AnalyzeAsync(GetSwitch(union, valueType, sections + " case null: break;"));

        diagnostics.Select(x => x.GetMessage()).Should().Equal("'null' case in 'Shape' is not handled.");
        diagnosticsWithNull.Should().BeEmpty();
    }

    [Test]
    [Arguments("case Circle: break; case { }: break;")]
    [Arguments("case Circle: break; case not null: break;")]
    public async Task Analyze_When_ClosedTypeIsTestedForNull_Then_NullShouldBeAskedForAsTheCompilerDoesForASwitchExpression(string sections)
    {
        // A pattern that tests for null tells nullable analysis that the value can be null, whatever its type says.
        var diagnostics = await GeneratorRunner.AnalyzeAsync(GetSwitch(ClosedType, "Shape", sections));

        diagnostics.Select(x => x.GetMessage()).Should().Equal("'null' case in 'Shape' is not handled.");
    }

    [Test]
    [Arguments(StructUnion, "Shape")]
    [Arguments(UnionDeclaration, "Shape")]
    [Arguments(ClosedType, "Shape")]
    [Arguments("#nullable disable\n" + ClosedType, "Shape")]
    [Arguments("public readonly record struct Empty; " + Cases + "public readonly partial struct Shape : IDefaultCase<Empty>, ICase<Circle>, ICase<Square>, ICase<Polygon>;", "Shape")]
    public async Task Analyze_When_NullableAnalysisDoesNotSayThatValueCanBeNull_Then_NullShouldNotBeAskedFor(string union, string valueType)
    {
        var sections = "case Circle: break; case Square: break; case Polygon: break;" + (union.Contains("Empty") ? " case Empty: break;" : string.Empty);

        var diagnostics = await GeneratorRunner.AnalyzeAsync(GetSwitch(union, valueType, sections));

        diagnostics.Should().BeEmpty();
    }

    [Test]
    [Arguments("public union Problem(Exception, InvalidOperationException, int);")]
    [Arguments("public readonly partial struct Problem : ICase<Exception>, ICase<InvalidOperationException>, ICase<int>;")]
    public async Task Analyze_When_CasesAreRelatedTypes_Then_ACaseShouldAlsoBeHandledByATypeItConvertsTo(string union)
    {
        var baseHandled = await GeneratorRunner.AnalyzeAsync(GetSwitch(union, "Problem", "case Exception: break; case int: break;"));
        var derivedHandled = await GeneratorRunner.AnalyzeAsync(GetSwitch(union, "Problem", "case InvalidOperationException: break; case int: break;"));

        baseHandled.Should().BeEmpty();
        derivedHandled.Select(x => x.GetMessage()).Should().Equal("'Exception' case in 'Problem' is not handled.");
    }

    [Test]
    public async Task Analyze_When_ClosedTypeDerivesFromAClosedType_Then_ItShouldHandleItsCasesAsAWholeOrOneByOne()
    {
        const string animals = "public closed record Animal; public closed record Mammal : Animal; public sealed record Dog : Mammal; public sealed record Cat : Mammal; public sealed record Fish : Animal;";

        var asAWhole = await GeneratorRunner.AnalyzeAsync(GetSwitch(animals, "Animal", "case Mammal: break; case Fish: break;"));
        var oneByOne = await GeneratorRunner.AnalyzeAsync(GetSwitch(animals, "Animal", "case Dog: break; case Cat: break; case Fish: break;"));
        var incomplete = await GeneratorRunner.AnalyzeAsync(GetSwitch(animals, "Animal", "case Dog: break; case Fish: break;"));
        var innerIncomplete = await GeneratorRunner.AnalyzeAsync(GetSwitch(animals, "Mammal", "case Dog: break;"));

        asAWhole.Should().BeEmpty();
        oneByOne.Should().BeEmpty();
        incomplete.Select(x => x.GetMessage()).Should().Equal("'Cat' case in 'Animal' is not handled.");
        innerIncomplete.Select(x => x.GetMessage()).Should().Equal("'Cat' case in 'Mammal' is not handled.");
    }

    [Test]
    [Arguments("public closed record Attempt<TValue>; public sealed record Success<TValue>(TValue Value) : Attempt<TValue>; public sealed record Failure<TValue>(string Reason) : Attempt<TValue>;", "Attempt<int>", "Success<int>", "Failure<int>")]
    [Arguments("public sealed record Failure(string Reason); public union Attempt<TValue>(TValue, Failure);", "Attempt<int>", "int", "Failure")]
    [Arguments("public readonly record struct Success<TValue>(TValue Value); public sealed record Failure(string Reason); public readonly partial struct Attempt<TValue> : ICase<Success<TValue>>, ICase<Failure>;", "Attempt<int>", "Success<int>", "Failure")]
    public async Task Analyze_When_UnionIsGeneric_Then_ItsConstructedCasesShouldBeAskedFor(string union, string valueType, string firstCase, string secondCase)
    {
        var incomplete = await GeneratorRunner.AnalyzeAsync(GetSwitch(union, valueType, $"case {firstCase}: break;"));
        var complete = await GeneratorRunner.AnalyzeAsync(GetSwitch(union, valueType, $"case {firstCase}: break; case {secondCase}: break;"));

        incomplete.Select(x => x.GetMessage()).Should().Equal($"'{secondCase}' case in '{valueType}' is not handled.");
        complete.Should().BeEmpty();
    }

    [Test]
    public async Task Analyze_When_SwitchIsInGenericCode_Then_CasesThatDependOnTheTypeParameterShouldBeAskedFor()
    {
        const string source = """
            public sealed record Failure(string Reason);
            public union Attempt<TValue>(TValue, Failure);

            public static class Program
            {
                public static void Run<TValue>(Attempt<TValue> value)
                {
                    switch (value)
                    {
                        case Failure:
                            break;
                    }
                }
            }
            """;

        var diagnostics = await GeneratorRunner.AnalyzeAsync(source);

        diagnostics.Select(x => x.GetMessage()).Should().Equal("'TValue' case in 'Attempt<TValue>' is not handled.");
    }

    [Test]
    [Arguments(StructUnion)]
    [Arguments(UnionDeclaration)]
    [Arguments(ClosedType)]
    public async Task Analyze_When_ACaseThrowsNotImplementedException_Then_ItShouldBeReportedAsNotImplemented(string union)
    {
        var diagnostics = await GeneratorRunner.AnalyzeAsync(GetSwitch(union, "Shape", "case Circle: break; case Square: throw new NotImplementedException(); case Polygon: break;"));

        var diagnostic = diagnostics.Should().ContainSingle().Which;
        diagnostic.Id.Should().Be("SDU0011");
        diagnostic.GetMessage().Should().Contain("Square");
    }

    private static string GetSwitch(string union, string valueType, string sections)
    {
        return union + $$"""

            public static class Program
            {
                public static void Run({{valueType}} value)
                {
                    switch (value)
                    {
                        {{sections}}
                    }
                }
            }
            """;
    }
}
