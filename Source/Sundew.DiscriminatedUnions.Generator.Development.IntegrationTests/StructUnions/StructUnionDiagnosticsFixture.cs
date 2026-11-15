// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructUnionDiagnosticsFixture.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Development.IntegrationTests.StructUnions;

using System.Linq;
using AwesomeAssertions;
using Microsoft.CodeAnalysis;
using Sundew.DiscriminatedUnions.Generator.Development.IntegrationTests.Unions;

public class StructUnionDiagnosticsFixture
{
    private const string Cases = """
        public readonly record struct Empty;
        public readonly record struct Unit;
        public readonly record struct Count(int Value);
        public readonly record struct Range(long From, long To);
        public sealed record Failure(string Reason);
        public sealed record Problem(string Title);

        """;

    [Test]
    [Arguments("public readonly struct Union : ICase<Count>, ICase<Range>;")]
    [Arguments("public readonly partial record struct Union : ICase<Count>, ICase<Range>;")]
    [Arguments("public ref partial struct Union : ICase<Count>, ICase<Range>;")]
    public void Run_When_UnionIsNotAPartialStruct_Then_ItShouldBeReportedAndNothingGenerated(string union)
    {
        var result = GeneratorRunner.Run(Cases + union);

        result.GeneratorDiagnostics.Select(x => x.Id).Should().Equal("GDU0002");
        result.GeneratedSources.Should().BeEmpty();
    }

    [Test]
    public void Run_When_AllCasesAreReferenceTypes_Then_UnionDeclarationShouldBeSuggestedAndNothingGenerated()
    {
        var result = GeneratorRunner.Run(Cases + "public readonly partial struct Union : ICase<Failure>, ICase<Problem>, ICase<string>;");

        var diagnostic = result.GeneratorDiagnostics.Should().ContainSingle().Which;
        diagnostic.Id.Should().Be("GDU0003");
        diagnostic.Severity.Should().Be(DiagnosticSeverity.Error);
        diagnostic.GetMessage().Should().Contain("Union").And.Contain("Failure, Problem, string");
        result.GeneratedSources.Should().BeEmpty();
    }

    [Test]
    public void Run_When_SomeCasesAreStructs_Then_UnionDeclarationShouldNotBeSuggested()
    {
        var result = GeneratorRunner.Run(Cases + "public readonly partial struct Union : ICase<Failure>, ICase<Problem>, ICase<Count>;");

        result.GeneratorDiagnostics.Should().BeEmpty();
        result.GeneratedSources.Should().ContainSingle();
    }

    [Test]
    public void Run_When_MoreThanOneCaseIsDefault_Then_ItShouldBeReportedAndNothingGenerated()
    {
        var result = GeneratorRunner.Run(Cases + "public readonly partial struct Union : IDefaultCase<Empty>, IDefaultCase<Unit>, ICase<Count>;");

        var diagnostic = result.GeneratorDiagnostics.Should().ContainSingle().Which;
        diagnostic.Id.Should().Be("GDU0004");
        diagnostic.GetMessage().Should().Contain("Empty, Unit");
        result.GeneratedSources.Should().BeEmpty();
    }

    [Test]
    [Arguments("Count")]
    [Arguments("Failure")]
    [Arguments("int")]
    public void Run_When_DefaultCaseHasPayload_Then_ItShouldBeReportedAndNothingGenerated(string defaultCase)
    {
        var result = GeneratorRunner.Run(Cases + $"public readonly partial struct Union : IDefaultCase<{defaultCase}>, ICase<Range>;");

        var diagnostic = result.GeneratorDiagnostics.Should().ContainSingle().Which;
        diagnostic.Id.Should().Be("GDU0005");
        diagnostic.GetMessage().Should().Contain(defaultCase);
        result.GeneratedSources.Should().BeEmpty();
    }

    [Test]
    public void Run_When_CaseIsTypeParameter_Then_ItShouldBeReportedAndNothingGenerated()
    {
        var result = GeneratorRunner.Run("public readonly partial struct Union<TItem> : ICase<TItem>;");

        var diagnostic = result.GeneratorDiagnostics.Should().ContainSingle().Which;
        diagnostic.Id.Should().Be("GDU0006");
        diagnostic.GetMessage().Should().Contain("TItem");
        result.GeneratedSources.Should().BeEmpty();
    }

    [Test]
    public void Run_When_UnionIsInvalid_Then_DiagnosticShouldBeLocatedAtTheNameOfTheUnion()
    {
        const string source = Cases + "public readonly struct Union : ICase<Count>, ICase<Range>;";

        var result = GeneratorRunner.Run(source);

        var sourceSpan = result.GeneratorDiagnostics.Should().ContainSingle().Which.Location.SourceSpan;
        source.Substring(sourceSpan.Start, sourceSpan.Length).Should().Be("Union");
    }
}
