﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionsMustBeAbstractAnalyzerTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Test.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.DimensionalUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DimensionalUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DimensionalUnionSwitchWarningSuppressor>;

[TestClass]
public class DiscriminatedUnionsMustBeAbstractAnalyzerTests
{
    [TestMethod]
    public async Task
        Given_DiscriminatedUnion_When_ItIsNotAbstract_Then_ClassDiscriminatedUnionsMustBeAbstractIsReported()
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
            VerifyCS.Diagnostic(DimensionalUnionsAnalyzer.ClassUnionsMustBeAbstractRule)
                .WithArguments("Unions.Result")
                .WithSpan(13, 1, 19, 2));
    }
}