// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructUnionGeneratorFixture.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Development.IntegrationTests.StructUnions;

using System.Linq;
using AwesomeAssertions;
using Sundew.DiscriminatedUnions.Generator.Development.IntegrationTests.Unions;

public class StructUnionGeneratorFixture
{
    private const string Cases = """
        public readonly record struct Empty;
        public readonly record struct Count(int Value);
        public readonly record struct Range(long From, long To);
        public readonly record struct Named(string Name);
        public sealed record Failure(string Reason);
        public sealed record Problem(string Title);

        """;

    [Test]
    public void Run_Then_GeneratedSourceShouldCompileWithoutWarnings()
    {
        var result = GeneratorRunner.Run(Cases + """
            public readonly partial struct WithDefault : IDefaultCase<Empty>, ICase<Count>, ICase<Range>, ICase<Named>;
            public readonly partial struct WithoutDefault : ICase<Empty>, ICase<Count>, ICase<Named>, ICase<Failure>, ICase<Problem>;
            public readonly partial struct Generic<TItem> : IDefaultCase<Empty>, ICase<List<TItem>>, ICase<KeyValuePair<TItem, int>>;
            public partial class Outer<TItem> { internal partial struct Inner : ICase<int>, ICase<TItem[]>; }
            """);

        result.GeneratorDiagnostics.Should().BeEmpty();
        result.CompilationDiagnostics.Should().BeEmpty();
        result.GeneratedSources.Select(x => x.HintName).Should().Equal(
            "WithDefault.generated.cs",
            "WithoutDefault.generated.cs",
            "Generic{TItem}.generated.cs",
            "Outer{TItem}.Inner.generated.cs");
    }

    [Test]
    [Arguments("[DiscriminatedUnion(true)]")]
    [Arguments("[DiscriminatedUnion(disableGenerator: true)]")]
    public void Run_When_GeneratorIsDisabled_Then_NothingShouldBeGenerated(string attribute)
    {
        var result = GeneratorRunner.Run(Cases + attribute + " public readonly partial struct Manual : ICase<Count>, ICase<Range>;");

        result.GeneratorDiagnostics.Should().BeEmpty();
        result.CompilationDiagnostics.Should().BeEmpty();
        result.GeneratedSources.Should().BeEmpty();
    }

    [Test]
    [Arguments("[DiscriminatedUnion]")]
    [Arguments("[DiscriminatedUnion(false)]")]
    [Arguments("[DiscriminatedUnion(disableGenerator: false)]")]
    public void Run_When_GeneratorIsNotDisabled_Then_MembersShouldBeGenerated(string attribute)
    {
        const string Union = " public readonly partial struct Size : ICase<Count>, ICase<Range>;";

        var result = GeneratorRunner.Run(Cases + attribute + Union);

        result.GeneratorDiagnostics.Should().BeEmpty();
        result.CompilationDiagnostics.Should().BeEmpty();
        result.GeneratedSources.Should().NotBeEmpty().And.Equal(GeneratorRunner.Run(Cases + Union).GeneratedSources);
    }

    [Test]
    public void Run_When_StructDeclaresNoCases_Then_NothingShouldBeGenerated()
    {
        var result = GeneratorRunner.Run("""
            public readonly partial struct NotAUnion : IEquatable<NotAUnion>
            {
                public bool Equals(NotAUnion other) => true;
            }
            """);

        result.GeneratorDiagnostics.Should().BeEmpty();
        result.GeneratedSources.Should().BeEmpty();
    }
}
