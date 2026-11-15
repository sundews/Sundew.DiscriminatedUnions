// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionGeneratorFixture.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Development.IntegrationTests;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AwesomeAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Sundew.Base.IO;
using Sundew.DiscriminatedUnions.Generator.Development.IntegrationTests.Unions;
using Sundew.Testing.CodeAnalysis;
using VerifyTUnit;

public class DiscriminatedUnionGeneratorFixture
{
    [Test]
    [Arguments("netstandard2.0")]
    [Arguments("net8.0")]
    [Arguments("net10.0")]
    public Task VerifyGeneratedSources(string targetFramework)
    {
        var project = new CSharpProject(Paths.FindPathUpwards("Sundew.DiscriminatedUnions.Development.Tester")!, null, new Paths("bin", "obj"));
        var compilation = project.Compile();
        GeneratorDriver generatorDriver = CSharpGeneratorDriver.Create(new DiscriminatedUnionGenerator());
        generatorDriver = generatorDriver.WithUpdatedAnalyzerConfigOptions(new TestAnalyzerConfigOptionsProvider(new DictionaryAnalyzerConfigOptions(new Dictionary<string, string> { { "build_property.TargetFramework", targetFramework } })));
        generatorDriver = generatorDriver.RunGenerators(compilation);

        var verifySettings = new VerifyTests.VerifySettings();
        verifySettings.UseFileName($"DUC.V.{targetFramework}");
        return Verifier.Verify(generatorDriver, verifySettings);
    }

    [Test]
    [Arguments("[DiscriminatedUnion(true)]")]
    [Arguments("[DiscriminatedUnion(disableGenerator: true)]")]
    public void Run_When_GeneratorIsDisabled_Then_NothingShouldBeGenerated(string attribute)
    {
        var result = GeneratorRunner.Run(GetUnion(attribute));

        result.GeneratorDiagnostics.Should().BeEmpty();
        result.CompilationDiagnostics.Should().BeEmpty();
        result.GeneratedSources.Should().BeEmpty();
    }

    [Test]
    [Arguments("[DiscriminatedUnion(false)]")]
    [Arguments("[DiscriminatedUnion(disableGenerator: false)]")]
    public void Run_When_GeneratorIsNotDisabled_Then_SourcesShouldBeGenerated(string attribute)
    {
        var result = GeneratorRunner.Run(GetUnion(attribute));

        result.GeneratorDiagnostics.Should().BeEmpty();
        result.GeneratedSources.Should().NotBeEmpty().And.Equal(GeneratorRunner.Run(GetUnion("[DiscriminatedUnion]")).GeneratedSources);
    }

    [Test]
    [Arguments("[DiscriminatedUnion(GeneratorFeatures.Segregate)]")]
    [Arguments("[DiscriminatedUnion(generatorFeatures: GeneratorFeatures.Segregate)]")]
    [Arguments("[DiscriminatedUnion(Sundew.DiscriminatedUnions.GeneratorFeatures.Segregate)]")]
    public void Run_When_SegregateIsRequested_Then_SegregationShouldBeGenerated(string attribute)
    {
        var result = GeneratorRunner.Run(GetUnion(attribute));

        result.GeneratorDiagnostics.Should().BeEmpty();
        result.GeneratedSources.Select(x => x.HintName).Should().Contain(x => x.Contains("ShapeSegregation"));
    }

    [Test]
    public void Run_When_CasesInDifferentNamespacesShareAName_Then_EachShouldGetItsOwnSource()
    {
        const string union = "[DiscriminatedUnion] public abstract partial record Result { public sealed partial record Success : Result; } public sealed partial record Failure : Result;";

        var result = GeneratorRunner.Run($"namespace First {{ {union} }} namespace Second {{ {union} }} namespace Third {{ {union} }}");

        result.GeneratorDiagnostics.Should().BeEmpty();
        result.CompilationDiagnostics.Should().BeEmpty();
        result.GeneratedSources.Select(x => x.HintName).Should().BeEquivalentTo(
            "First.Result.generated.cs",
            "First.Result.Success.generated.cs",
            "First.Failure.generated.cs",
            "Second.Result.generated.cs",
            "Second.Result.Success.generated.cs",
            "Second.Failure.generated.cs",
            "Third.Result.generated.cs",
            "Third.Result.Success.generated.cs",
            "Third.Failure.generated.cs");
    }

    [Test]
    public void Run_When_CaseHasDefaultValues_Then_FactoryShouldHaveTheSameDefaultValues()
    {
        var result = GeneratorRunner.Run("""
            namespace Shapes;

            [DiscriminatedUnion]
            public abstract partial record Shape
            {
                public sealed partial record Label(string Text = "x y", char Symbol = 'c', double Size = 1.5, decimal Price = 2.5m) : Shape;
            }
            """);

        result.GeneratorDiagnostics.Should().BeEmpty();
        result.CompilationDiagnostics.Should().BeEmpty();
        result.GeneratedSources.Select(x => x.Source).Should().Contain(x => x.Contains("""_Label(string text = "x y", char symbol = 'c', double size = 1.5, decimal price = 2.5M)"""));
    }

    private static string GetUnion(string attribute)
    {
        return $$"""
            namespace Shapes;

            {{attribute}}
            public abstract partial record Shape
            {
                public sealed partial record Circle(double Radius) : Shape;

                public sealed partial record Square(double Side) : Shape;
            }
            """;
    }
}
