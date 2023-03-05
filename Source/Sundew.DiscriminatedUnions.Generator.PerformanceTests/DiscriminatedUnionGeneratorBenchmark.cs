namespace Sundew.DiscriminatedUnions.Generator.PerformanceTests;

extern alias baseline;

using BaselineDiscriminatedUnionGenerator = baseline::Sundew.DiscriminatedUnions.Generator.DiscriminatedUnionGenerator;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Sundew.CodeAnalysis.Testing;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net48, baseline: true)]
[SimpleJob(RuntimeMoniker.Net70)]
public class DiscriminatedUnionGeneratorBenchmark
{
    private readonly CSharpCompilation compilation;

    public DiscriminatedUnionGeneratorBenchmark()
    {
        var project = new Project(DemoProjectInfo.GetPath("Sundew.DiscriminatedUnions.Tester"), new Paths(), "bin", "obj");
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