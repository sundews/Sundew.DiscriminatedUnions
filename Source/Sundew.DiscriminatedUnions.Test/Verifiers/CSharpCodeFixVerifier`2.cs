// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CSharpCodeFixVerifier`2.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Sundew.DiscriminatedUnions.TestData;

public static partial class CSharpCodeFixVerifier<TAnalyzer, TCodeFix, TSuppressor>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : CodeFixProvider, new()
    where TSuppressor : DiagnosticSuppressor, new()
{
    /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.Diagnostic()"/>
    public static DiagnosticResult Diagnostic()
        => Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<TAnalyzer, TCodeFix, MSTestVerifier>.Diagnostic();

    /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.Diagnostic(string)"/>
    public static DiagnosticResult Diagnostic(string diagnosticId)
        => Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<TAnalyzer, TCodeFix, MSTestVerifier>.Diagnostic(diagnosticId);

    /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.Diagnostic(DiagnosticDescriptor)"/>
    public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
        => Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<TAnalyzer, TCodeFix, MSTestVerifier>.Diagnostic(descriptor);

    /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.VerifyAnalyzerAsync(string, DiagnosticResult[])"/>
    public static async Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
    {
        var test = new Test
        {
            TestCode = source,
        };

        test.TestCode = CSharpVerifierHelper.RequiredTypes;
        test.SolutionTransforms.Add((solution, id) =>
            solution
                .AddMetadataReference(id, MetadataReference.CreateFromFile(typeof(DiscriminatedUnion).Assembly.Location))
                .AddMetadataReference(id, MetadataReference.CreateFromFile(typeof(IExpression).Assembly.Location)));
        test.ExpectedDiagnostics.AddRange(expected);
        await test.RunAsync(CancellationToken.None);
    }

    /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.VerifyCodeFixAsync(string, string)"/>
    public static async Task VerifyCodeFixAsync(string source, string fixedSource)
        => await VerifyCodeFixAsync(source, DiagnosticResult.EmptyDiagnosticResults, fixedSource);

    /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.VerifyCodeFixAsync(string, DiagnosticResult, string)"/>
    public static async Task VerifyCodeFixAsync(string source, DiagnosticResult expected, string fixedSource)
        => await VerifyCodeFixAsync(source, new[] { expected }, fixedSource);

    /// <inheritdoc cref="CodeFixVerifier{TAnalyzer, TCodeFix, TTest, TVerifier}.VerifyCodeFixAsync(string, DiagnosticResult[], string)"/>
    public static async Task VerifyCodeFixAsync(string source, DiagnosticResult[] expected, string fixedSource, DiagnosticResult[]? expectedAfter, int? numberIterations = null)
    {
        var test = new Test
        {
            TestCode = source,
            FixedCode = fixedSource,
        };

        test.TestCode = CSharpVerifierHelper.RequiredTypes;
        test.FixedCode = CSharpVerifierHelper.RequiredTypes;
        test.SolutionTransforms.Add((solution, id) =>
            solution
                .AddMetadataReference(id, MetadataReference.CreateFromFile(typeof(DiscriminatedUnion).Assembly.Location))
                .AddMetadataReference(id, MetadataReference.CreateFromFile(typeof(IExpression).Assembly.Location)));
        test.ExpectedDiagnostics.AddRange(expected);
        if (expectedAfter != null)
        {
            test.FixedState.ExpectedDiagnostics.AddRange(expectedAfter);
        }

        test.NumberOfIncrementalIterations = numberIterations;
        test.NumberOfFixAllIterations = numberIterations;

        await test.RunAsync(CancellationToken.None);
    }

    public static Task VerifyCodeFixAsync(string source, DiagnosticResult[] expected, string fixedSource, bool diagnosticWillRemain = false, int? numberIterations = null)
    {
        return VerifyCodeFixAsync(source, expected, fixedSource, diagnosticWillRemain ? expected : null, numberIterations);
    }
}