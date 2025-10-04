// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionGeneratorBenchmark.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.PerformanceTests;

extern alias baseline;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Sundew.Base.IO;
using Sundew.Testing.CodeAnalysis;

using BaselineDiscriminatedUnionGenerator = baseline::Sundew.DiscriminatedUnions.Generator.DiscriminatedUnionGenerator;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net48, baseline: true)]
[SimpleJob(RuntimeMoniker.Net80)]
public class DiscriminatedUnionGeneratorBenchmark
{
    private readonly Compilation compilation;

    public DiscriminatedUnionGeneratorBenchmark()
    {
        var project = new CSharpProject(Paths.FindPathUpwards("Sundew.DiscriminatedUnions.Tester")!, excludePaths: new Paths("bin", "obj"));
        this.compilation = project.Compile();
    }

    [Benchmark(Baseline = true)]
    public object BaselineGenerator()
    {
        GeneratorDriver generatorDriver = CSharpGeneratorDriver.Create(new BaselineDiscriminatedUnionGenerator());

        var driver = generatorDriver.RunGenerators(this.compilation);
        var result = driver.GetRunResult();
        return result;
    }

    [Benchmark]
    public object WorkInProgressGenerator()
    {
        GeneratorDriver generatorDriver = CSharpGeneratorDriver.Create(new DiscriminatedUnionGenerator());

        var driver = generatorDriver.RunGenerators(this.compilation);
        var result = driver.GetRunResult();
        return result;
    }
}