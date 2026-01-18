// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionGeneratorFixture.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Development.IntegrationTests;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Sundew.Base.IO;
using Sundew.Testing.CodeAnalysis;
using VerifyXunit;
using Xunit;

public class DiscriminatedUnionGeneratorFixture
{
    [Theory]
    [InlineData("netstandard2.0")]
    [InlineData("net8.0")]
    [InlineData("net10.0")]
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
}
