// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionsMustBeAbstractAnalyzerTests.cs" company="Hukano">
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
    public class DiscriminatedUnionsMustBeAbstractAnalyzerTests
    {
        [TestMethod]
        public async Task
            Given_DiscriminatedUnion_When_ItIsNotAbstract_Then_ClassDiscriminatedUnionsMustBeAbstractIsReported()
        {
            var test = $@"#nullable enable
{TestData.Usings}
namespace ConsoleApplication1
{{
    [Sundew.DiscriminatedUnions.DiscriminatedUnion]
    public record Result
    {{
        private protected Result()
        {{ }} 

        public sealed record Success : Result;

        public sealed record Warning(string Message) : Result;
    }}
}}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.ClassDiscriminatedUnionsMustBeAbstractRule)
                    .WithArguments("ConsoleApplication1.Result")
                    .WithSpan(12, 5, 21, 6));
        }
    }
}