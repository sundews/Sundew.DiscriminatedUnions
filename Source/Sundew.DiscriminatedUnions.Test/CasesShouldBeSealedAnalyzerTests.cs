// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CasesShouldBeSealedAnalyzerTests.cs" company="Hukano">
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
public class CasesShouldBeSealedAnalyzerTests
{
    [TestMethod]
    public async Task Given_Union_When_CaseIsNotSealed_Then_CasesShouldBeSealedIsReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}
namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Result
{{
    private protected Result()
    {{ }}

    public record Success : Result;

    public sealed record Warning(string Message) : Result;

    public sealed record Error(int Code) : Result;
}}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DimensionalUnionsAnalyzer.CasesShouldBeSealedRule)
                .WithArguments("Unions.Result.Success")
                .WithSpan(19, 5, 19, 36));
    }
}