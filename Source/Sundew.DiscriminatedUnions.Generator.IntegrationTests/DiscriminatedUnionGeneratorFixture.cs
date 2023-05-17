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
using NUnit.Framework;
using Sundew.Testing.CodeAnalysis;
using Sundew.Testing.IO;
using VerifyNUnit;

[TestFixture]
public class DiscriminatedUnionGeneratorFixture
{
    [Test]
    public Task VerifyGeneratedSources()
    {
        var project = new CSharpProject(Paths.FindPathUpwards("Sundew.DiscriminatedUnions.Tester"), new Paths(), new Paths("bin", "obj"), null);
        var compilation = project.Compile();
        GeneratorDriver generatorDriver = CSharpGeneratorDriver.Create(new DiscriminatedUnionGenerator());

        generatorDriver = generatorDriver.RunGenerators(compilation);

        return Verifier.Verify(generatorDriver);
    }
}