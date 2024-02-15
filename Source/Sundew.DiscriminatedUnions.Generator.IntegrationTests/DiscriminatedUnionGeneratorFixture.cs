// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionGeneratorFixture.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.IntegrationTests;

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Sundew.Base.IO;
using Sundew.Testing.CodeAnalysis;
using VerifyXunit;
using Xunit;

public class DiscriminatedUnionGeneratorFixture
{
    [Fact]
    public Task VerifyGeneratedSources()
    {
        var project = new CSharpProject(Paths.FindPathUpwards("Sundew.DiscriminatedUnions.Tester")!, null, new Paths("bin", "obj"));
        var compilation = project.Compile();
        GeneratorDriver generatorDriver = CSharpGeneratorDriver.Create(new DiscriminatedUnionGenerator());

        generatorDriver = generatorDriver.RunGenerators(compilation);

        return Verifier.Verify(generatorDriver);
    }
}