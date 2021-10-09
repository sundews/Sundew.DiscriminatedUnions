// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionTypeAnalyzerTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test
{
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sundew.DiscriminatedUnions.Analyzer;
    using VerifyCS = Sundew.DiscriminatedUnions.Test.CSharpCodeFixVerifier<
        Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionsAnalyzer,
        Sundew.DiscriminatedUnions.CodeFixes.SundewDiscriminatedUnionsCodeFixProvider,
        Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionSwitchWarningSuppressor>;

    [TestClass]
    public class DiscriminatedUnionTypeAnalyzerTests
    {
        [TestMethod]
        public async Task Given_DiscriminatedUnion_When_ConstructorIsNotPrivateAndCaseIsNotSealed_Then_DiscriminatedUnionCanOnlyHavePrivateProtectedConstructorsAndCasesShouldBeSealedAreReported()
        {
            var test = $@"#nullable enable
{TestData.Usings}
namespace ConsoleApplication1
{{
    [Sundew.DiscriminatedUnions.DiscriminatedUnion]
    public abstract record Result
    {{
        protected Result()
        {{ }}

        public record Success : Result;

        public sealed record Warning(string Message) : Result;
    }}

    public sealed record Error(int Code) : Result;
}}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.DiscriminatedUnionCanOnlyHavePrivateConstructorsDiagnosticId)
                    .WithArguments(TestData.ConsoleApplication1Result)
                    .WithSpan(15, 9, 16, 12),
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.CasesShouldBeSealedDiagnosticId)
                    .WithArguments("ConsoleApplication1.Result.Success")
                    .WithSpan(18, 9, 18, 40),
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.CasesShouldBeNestedDiagnosticId)
                    .WithArguments("ConsoleApplication1.Error", TestData.ConsoleApplication1Result)
                    .WithSpan(23, 5, 23, 51));
        }

        [TestMethod]
        public async Task Given_DiscriminatedUnion_When_ConstructorIsNotPrivateAndCaseIsNotSealed_Then_DiscriminatedUnionCanOnlyHavePrivateProtectedConstructorsAndCasesShouldBeSealedAreReported2()
        {
            var test = $@"#nullable enable
{TestData.Usings}
namespace ConsoleApplication1
{{
    [Sundew.DiscriminatedUnions.DiscriminatedUnion]
    public abstract record Result
    {{
        public sealed record Success : Result;

        public sealed record Warning(string Message) : Result;
    }}
}}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.MustHavePrivateConstructorRule)
                    .WithArguments("ConsoleApplication1.Result", "ConsoleApplication1.Result.Result()")
                    .WithSpan(12, 5, 18, 6));
        }
    }
}