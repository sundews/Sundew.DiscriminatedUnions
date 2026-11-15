// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionGeneratorBenchmark.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.PerformanceTests;

extern alias baseline;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains;
using BenchmarkDotNet.Toolchains.CsProj;
using BenchmarkDotNet.Toolchains.DotNetCli;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Sundew.Base.IO;
using Sundew.Testing.CodeAnalysis;

using BaselineDiscriminatedUnionGenerator = baseline::Sundew.DiscriminatedUnions.Generator.DiscriminatedUnionGenerator;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net48, baseline: true)]
[SimpleJob(RuntimeMoniker.Net10_0)]
[Config(typeof(Net11Job))]
public class DiscriminatedUnionGeneratorBenchmark
{
    private readonly Compilation compilation;

    public DiscriminatedUnionGeneratorBenchmark()
    {
        var project = new CSharpProject(Paths.FindPathUpwards("Sundew.DiscriminatedUnions.Development.Tester")!, excludePaths: new Paths("bin", "obj"));
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

    // BenchmarkDotNet 0.15.8 has no RuntimeMoniker for .NET 11 (0.16 adds Net11_0), so the job for the net11.0 target framework is put together by hand.
    private sealed class Net11Job : ManualConfig
    {
        public Net11Job()
        {
            var runtime = CoreRuntime.CreateForNewVersion("net11.0", ".NET 11.0");
            var toolchain = CsProjCoreToolchain.From(new NetCoreAppSettings(runtime.MsBuildMoniker, null, runtime.Name));

            // BenchmarkDotNet 0.15.8 looks up the version of a runtime through its RuntimeMoniker in two places and throws for a runtime without one:
            // in the SDK check of CsProjCoreToolchain, which Toolchain does not have, and when it decides whether the jobs can be built in parallel.
            this.AddJob(Job.Default
                .WithRuntime(runtime)
                .WithToolchain(new Toolchain(toolchain.Name, toolchain.Generator, toolchain.Builder, toolchain.Executor))
                .WithId(runtime.Name));
            this.WithOptions(ConfigOptions.DisableParallelBuild);
        }
    }
}