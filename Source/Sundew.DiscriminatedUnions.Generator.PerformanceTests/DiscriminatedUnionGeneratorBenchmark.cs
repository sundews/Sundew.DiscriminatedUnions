namespace Sundew.DiscriminatedUnions.Generator.PerformanceTests;

extern alias baseline;

using BaselineDiscriminatedUnionGenerator = baseline::Sundew.DiscriminatedUnions.Generator.DiscriminatedUnionGenerator;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Sundew.Testing.CodeAnalysis;
using Sundew.Testing.IO;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net48, baseline: true)]
[SimpleJob(RuntimeMoniker.Net80)]
public class DiscriminatedUnionGeneratorBenchmark
{
    private readonly Compilation compilation;

    public DiscriminatedUnionGeneratorBenchmark()
    {
        var project = new CSharpProject(Paths.FindPathUpwards("Sundew.DiscriminatedUnions.Tester"), excludePaths: new Paths("bin", "obj"));
        this.compilation = project.Compile();
    }

    [Benchmark(Baseline = true)]
    public object BaselineGenerator()
    {
        GeneratorDriver generatorDriver = CSharpGeneratorDriver.Create(new BaselineDiscriminatedUnionGenerator());

        var driver = generatorDriver.RunGenerators(compilation);
        var result = driver.GetRunResult();
        return result;
    }

    [Benchmark]
    public object WorkInProgressGenerator()
    {
        GeneratorDriver generatorDriver = CSharpGeneratorDriver.Create(new DiscriminatedUnionGenerator());

        var driver= generatorDriver.RunGenerators(compilation);
        var result = driver.GetRunResult();
        return result;
    }
}