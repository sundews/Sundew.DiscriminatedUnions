﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionGeneratorFixture.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.IntegrationTests;

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using Sundew.DiscriminatedUnions.Generator.IntegrationTests.Testing;
using VerifyNUnit;

[TestFixture]
public class DiscriminatedUnionGeneratorFixture
{
    [Test]
    public Task VerifyGeneratedSources()
    {
        var project = new Testing.Project(DemoProjectInfo.GetPath("Sundew.DiscriminatedUnions.Tester"), new Paths(), "bin", "obj");
        var compilation = project.Compile();
        GeneratorDriver generatorDriver = CSharpGeneratorDriver.Create(new DiscriminatedUnionGenerator());

        generatorDriver = generatorDriver.RunGenerators(compilation);

        return Verifier.Verify(generatorDriver);
    }
}