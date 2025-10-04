// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnionsMustBeAbstractAnalyzerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tests;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Development.Tests.Verifiers.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionSwitchWarningSuppressor>;

[TestClass]
public class UnionsMustBeAbstractAnalyzerTests
{
    [TestMethod]
    public async Task
        Given_Union_When_ItIsNotAbstract_Then_ClassDiscriminatedUnionsMustBeAbstractIsReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}
namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public record Result
{{
    public sealed record Success : Result;

    public sealed record Warning(string Message) : Result;
}}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.ClassUnionsMustBeAbstractRule)
                .WithArguments("Unions.Result")
                .WithSpan(13, 1, 19, 2));
    }
}