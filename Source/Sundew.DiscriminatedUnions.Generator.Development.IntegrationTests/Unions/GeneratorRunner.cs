// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneratorRunner.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Generator.Development.IntegrationTests.Unions;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Sundew.DiscriminatedUnions.Analyzer;
using Sundew.Testing.CodeAnalysis;

/// <summary>
/// Runs the generator on source code that is compiled against the framework the tests run on and the public Sundew.DiscriminatedUnions library.
/// </summary>
internal static class GeneratorRunner
{
    private const string LibraryFileName = "Sundew.DiscriminatedUnions.dll";
    private const string TrustedPlatformAssemblies = "TRUSTED_PLATFORM_ASSEMBLIES";
    private const string GlobalUsings = "global using System; global using System.Collections.Generic; global using Sundew.DiscriminatedUnions;";
    private const string TargetFrameworkProperty = "build_property.TargetFramework";
    private const string DefaultTargetFramework = "net11.0";

    // The generator and the analyzer contain private copies of the library types, which must not be mistaken for the library.
    private static readonly string[] ExcludedReferencePrefixes = ["Sundew.DiscriminatedUnions"];

    private static readonly ImmutableArray<MetadataReference> References = GetReferences();

    /// <summary>
    /// Gets the reference assembly of .NET Standard 2.0, in which structs have no fields.
    /// </summary>
    public static ImmutableArray<MetadataReference> NetStandard20References { get; } =
        [MetadataReference.CreateFromFile(Path.Combine(AppContext.BaseDirectory, "References", "netstandard2.0", "netstandard.dll"))];

    public static Result Run(string source, string targetFramework = DefaultTargetFramework)
    {
        return Run(source, targetFramework, References);
    }

    public static Result Run(string source, string targetFramework, ImmutableArray<MetadataReference> references)
    {
        var generatorDriver = RunGenerator(source, targetFramework, references, out var outputCompilation, out var generatorDiagnostics);
        var generatedSources = generatorDriver.GetRunResult().GeneratedTrees.Select(x => (Path.GetFileName(x.FilePath), x.ToString())).ToImmutableArray();
        return new Result(generatorDiagnostics, generatedSources, outputCompilation.GetDiagnostics().Where(x => x.Severity >= DiagnosticSeverity.Warning).ToImmutableArray());
    }

    /// <summary>
    /// Runs the generator on the specified source, compiles the result and loads it, so that the generated code can be executed.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The loaded assembly, if the source compiled, and the diagnostics of the generator and the compilation.</returns>
    public static LoadResult Load(string source)
    {
        var generatorDriver = RunGenerator(source, DefaultTargetFramework, References, out var outputCompilation, out var generatorDiagnostics);
        var generatedSources = generatorDriver.GetRunResult().GeneratedTrees.Select(x => (Path.GetFileName(x.FilePath), x.ToString())).ToImmutableArray();
        using var assemblyStream = new MemoryStream();
        var emitResult = outputCompilation.Emit(assemblyStream);
        var diagnostics = generatorDiagnostics.Concat(emitResult.Diagnostics.Where(x => x.Severity >= DiagnosticSeverity.Warning)).ToImmutableArray();
        if (!emitResult.Success)
        {
            return new LoadResult(null, generatedSources, diagnostics);
        }

        assemblyStream.Position = 0;
        return new LoadResult(new LibraryLoadContext().LoadFromStream(assemblyStream), generatedSources, diagnostics);
    }

    /// <summary>
    /// Runs the generator on the specified source and then the analyzers for class unions on the result.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The diagnostics reported by the analyzers.</returns>
    public static async Task<ImmutableArray<Diagnostic>> AnalyzeAsync(string source)
    {
        CSharpGeneratorDriver.Create(new DiscriminatedUnionGenerator()).RunGeneratorsAndUpdateCompilation(CreateCompilation(source, References), out var outputCompilation, out _);
        var analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new DiscriminatedUnionsAnalyzer(), new MakePartialMarkerAnalyzer(), new PopulateFactoryMethodsMarkerAnalyzer());
        return await outputCompilation.WithAnalyzers(analyzers).GetAnalyzerDiagnosticsAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Runs the generator on the specified source and then the analyzer and the suppressor for class unions, and gets the warnings that remain.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The ids of the warnings and errors of the compiler and the analyzer that are not suppressed.</returns>
    public static async Task<ImmutableArray<string>> GetWarningIdsAsync(string source)
    {
        CSharpGeneratorDriver.Create(new DiscriminatedUnionGenerator()).RunGeneratorsAndUpdateCompilation(CreateCompilation(source, References), out var outputCompilation, out _);
        var analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new DiscriminatedUnionsAnalyzer(), new DiscriminatedUnionSwitchWarningSuppressor());
        var diagnostics = await outputCompilation.WithAnalyzers(analyzers).GetAllDiagnosticsAsync().ConfigureAwait(false);
        return diagnostics.Where(x => x.Severity >= DiagnosticSeverity.Warning && !x.IsSuppressed).Select(x => x.Id).ToImmutableArray();
    }

    /// <summary>
    /// Runs the generator on the specified sources and then again after a change, and gets what happened to each source output the second time.
    /// An output that is cached was not written again, because what the generator found out about its union was the same as before.
    /// </summary>
    /// <param name="sources">The sources.</param>
    /// <param name="changedSources">The sources after the change, in the same order.</param>
    /// <returns>The reason each source output of the second run ended up the way it did.</returns>
    public static ImmutableArray<IncrementalStepRunReason> RunAgain(IReadOnlyList<string> sources, IReadOnlyList<string> changedSources)
    {
        GeneratorDriver generatorDriver = CSharpGeneratorDriver.Create(
            [new DiscriminatedUnionGenerator().AsSourceGenerator()],
            driverOptions: new GeneratorDriverOptions(IncrementalGeneratorOutputKind.None, trackIncrementalGeneratorSteps: true));
        generatorDriver = generatorDriver.WithUpdatedAnalyzerConfigOptions(new TestAnalyzerConfigOptionsProvider(new DictionaryAnalyzerConfigOptions(new Dictionary<string, string> { { TargetFrameworkProperty, DefaultTargetFramework } })));
        var syntaxTrees = sources.Select(x => CSharpSyntaxTree.ParseText(x)).ToArray();
        Compilation compilation = CreateCompilation(syntaxTrees.Prepend(CSharpSyntaxTree.ParseText(GlobalUsings)), References);
        generatorDriver = generatorDriver.RunGenerators(compilation);

        // A changed source replaces its syntax tree, the way an edit does. The other syntax trees stay the ones they were.
        for (var index = 0; index < sources.Count; index++)
        {
            if (sources[index] != changedSources[index])
            {
                compilation = compilation.ReplaceSyntaxTree(syntaxTrees[index], CSharpSyntaxTree.ParseText(changedSources[index]));
            }
        }

        return generatorDriver.RunGenerators(compilation).GetRunResult().Results.Single().TrackedOutputSteps
            .SelectMany(x => x.Value)
            .SelectMany(x => x.Outputs)
            .Select(x => x.Reason)
            .ToImmutableArray();
    }

    private static GeneratorDriver RunGenerator(string source, string targetFramework, ImmutableArray<MetadataReference> references, out Compilation outputCompilation, out ImmutableArray<Diagnostic> generatorDiagnostics)
    {
        GeneratorDriver generatorDriver = CSharpGeneratorDriver.Create(new DiscriminatedUnionGenerator());
        generatorDriver = generatorDriver.WithUpdatedAnalyzerConfigOptions(new TestAnalyzerConfigOptionsProvider(new DictionaryAnalyzerConfigOptions(new Dictionary<string, string> { { TargetFrameworkProperty, targetFramework } })));
        return generatorDriver.RunGeneratorsAndUpdateCompilation(CreateCompilation(source, references), out outputCompilation, out generatorDiagnostics);
    }

    private static CSharpCompilation CreateCompilation(string source, ImmutableArray<MetadataReference> references)
    {
        return CreateCompilation([CSharpSyntaxTree.ParseText(GlobalUsings), CSharpSyntaxTree.ParseText(source)], references);
    }

    private static CSharpCompilation CreateCompilation(IEnumerable<SyntaxTree> syntaxTrees, ImmutableArray<MetadataReference> references)
    {
        return CSharpCompilation.Create(
            "UnionTests",
            syntaxTrees,
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));
    }

    private static ImmutableArray<MetadataReference> GetReferences()
    {
        var trustedPlatformAssemblies = ((string)AppContext.GetData(TrustedPlatformAssemblies)!).Split(Path.PathSeparator);
        return trustedPlatformAssemblies
            .Where(x => !ExcludedReferencePrefixes.Any(prefix => Path.GetFileName(x).StartsWith(prefix, StringComparison.Ordinal)))
            .Append(Path.Combine(AppContext.BaseDirectory, LibraryFileName))
            .Select(x => (MetadataReference)MetadataReference.CreateFromFile(x))
            .ToImmutableArray();
    }

    /// <summary>
    /// The result of running the generator.
    /// </summary>
    /// <param name="GeneratorDiagnostics">The diagnostics reported by the generator.</param>
    /// <param name="GeneratedSources">The hint names and the contents of the generated sources.</param>
    /// <param name="CompilationDiagnostics">The warnings and errors of the compilation that includes the generated sources.</param>
    internal sealed record Result(
        ImmutableArray<Diagnostic> GeneratorDiagnostics,
        ImmutableArray<(string HintName, string Source)> GeneratedSources,
        ImmutableArray<Diagnostic> CompilationDiagnostics);

    /// <summary>
    /// The result of running the generator and loading what it and the source compile to.
    /// </summary>
    /// <param name="Assembly">The loaded assembly or <c>null</c>, if the source did not compile.</param>
    /// <param name="GeneratedSources">The hint names and the contents of the generated sources.</param>
    /// <param name="Diagnostics">The diagnostics reported by the generator, and the warnings and errors of the compilation that includes the generated sources.</param>
    internal sealed record LoadResult(
        Assembly? Assembly,
        ImmutableArray<(string HintName, string Source)> GeneratedSources,
        ImmutableArray<Diagnostic> Diagnostics);

    private sealed class LibraryLoadContext() : AssemblyLoadContext(nameof(LibraryLoadContext))
    {
        protected override Assembly? Load(AssemblyName assemblyName)
        {
            // The tests do not depend on the public library, so the runtime does not find it by itself.
            return assemblyName.Name == Path.GetFileNameWithoutExtension(LibraryFileName) ? this.LoadFromAssemblyPath(Path.Combine(AppContext.BaseDirectory, LibraryFileName)) : null;
        }
    }
}
