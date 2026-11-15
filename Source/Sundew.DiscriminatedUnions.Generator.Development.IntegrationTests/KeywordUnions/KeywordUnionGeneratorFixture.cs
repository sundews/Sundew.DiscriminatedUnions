// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeywordUnionGeneratorFixture.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Development.IntegrationTests.KeywordUnions;

using System.Linq;
using AwesomeAssertions;
using Sundew.DiscriminatedUnions.Generator.Development.IntegrationTests.Unions;

public class KeywordUnionGeneratorFixture
{
    private const string CasesProperty = "public static global::System.Collections.Generic.IReadOnlyList<global::System.Type> Cases { get; }";
    private const string CaseListInterface = "global::Sundew.DiscriminatedUnions.IDiscriminatedUnion";
    private const string Cases = """
        public readonly record struct Completed(int Code);
        public sealed record Failed(string Reason);

        """;

    [Test]
    public void Run_When_UnionIsPartial_Then_MembersShouldBeGeneratedAndCompileWithoutWarnings()
    {
        var result = GeneratorRunner.Run(Cases + """
            public partial union Completion(Completed, Failed);
            public partial union Generic<TItem>(Completed, List<TItem>);
            public partial class Outer<TItem> { public partial union Inner(Completed, TItem[]); }
            """);

        result.GeneratorDiagnostics.Should().BeEmpty();
        result.CompilationDiagnostics.Should().BeEmpty();
        result.GeneratedSources.Select(x => x.HintName).Should().Equal("Completion.generated.cs", "Generic{TItem}.generated.cs", "Outer{TItem}.Inner.generated.cs");
        result.GeneratedSources[0].Source.Should().Contain("partial union Completion : global::System.IEquatable<Completion>, global::Sundew.DiscriminatedUnions.IDiscriminatedUnion")
            .And.Contain("= new global::System.Type[] { typeof(global::Completed), typeof(global::Failed) };")
            .And.Contain("public static bool operator ==(Completion left, Completion right)")
            .And.Contain("public bool Equals(Completion other)")
            .And.Contain("public override int GetHashCode()")
            .And.Contain("public override string ToString()");
        result.GeneratedSources[1].Source.Should().Contain("typeof(global::System.Collections.Generic.List<TItem>)");
    }

    [Test]
    [Arguments("public union Completion(Completed, Failed);")]
    [Arguments("[DiscriminatedUnion(true)] public partial union Completion(Completed, Failed);")]
    [Arguments("[DiscriminatedUnion(disableGenerator: true)] public partial union Completion(Completed, Failed);")]
    public void Run_When_UnionIsNotPartialOrGeneratorIsDisabled_Then_NothingShouldBeGenerated(string union)
    {
        var result = GeneratorRunner.Run(Cases + union);

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
        const string Union = " public partial union Completion(Completed, Failed);";

        var source = Generate(attribute + Union);

        source.Should().Be(Generate(Union));
    }

    [Test]
    public void Run_When_UnionDeclaresToString_Then_ItShouldNotBeGenerated()
    {
        var source = Generate("public partial union Completion(Completed, Failed) { public override string ToString() => string.Empty; }");

        source.Should().NotContain("ToString").And.Contain("public bool Equals(Completion other)").And.Contain(CasesProperty);
    }

    [Test]
    public void Run_When_UnionDeclaresEquality_Then_EqualityShouldNotBeGenerated()
    {
        var source = Generate("""
            public partial union Completion(Completed, Failed)
            {
                public override bool Equals(object? obj) => false;

                public override int GetHashCode() => 0;
            }
            """);

        source.Should().Contain("partial union Completion : " + CaseListInterface)
            .And.NotContain("IEquatable")
            .And.NotContain("Equals")
            .And.NotContain("operator")
            .And.Contain("public override string ToString()");
    }

    [Test]
    public void Run_When_UnionDeclaresCases_Then_CasesShouldNotBeGenerated()
    {
        var source = Generate("public partial union Completion(Completed, Failed) { public static int Cases => 2; }");

        source.Should().Contain("partial union Completion : global::System.IEquatable<Completion>")
            .And.NotContain(CaseListInterface)
            .And.NotContain(CasesProperty);
    }

    [Test]
    [Arguments("net11.0", true)]
    [Arguments("net8.0", true)]
    [Arguments("net6.0", false)]
    [Arguments("netstandard2.0", false)]
    public void Run_Then_CaseListInterfaceShouldOnlyBeImplementedWhereItExists(string targetFramework, bool expectedResult)
    {
        var source = GeneratorRunner.Run(Cases + "public partial union Completion(Completed, Failed);", targetFramework).GeneratedSources.Single().Source;

        source.Contains(CaseListInterface).Should().Be(expectedResult);
        source.Should().Contain(CasesProperty);
    }

    private static string Generate(string union)
    {
        var result = GeneratorRunner.Run(Cases + union);
        result.GeneratorDiagnostics.Should().BeEmpty();
        result.CompilationDiagnostics.Should().BeEmpty();
        return result.GeneratedSources.Single().Source;
    }
}
