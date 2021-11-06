// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionsMustBeAbstractCodeFixTests.cs" company="Hukano">
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
    public class DiscriminatedUnionsMustBeAbstractCodeFixTests
    {
        [TestMethod]
        public async Task Given_DiscriminatedUnion_When_ItIsNotDeclaredAbstract_Then_ItShouldBeDeclaredAbstract()
        {
            var test = $@"{TestData.Usings}

namespace ConsoleApplication1
{{
    [Sundew.DiscriminatedUnions.DiscriminatedUnion]
    public record Result
    {{
        private Result()
        {{ }}

        public sealed record Success : Result;

        public sealed record Warning(string Message) : Result;

        public sealed record Error(int Code) : Result;
    }}
}}";

            var fixtest = $@"{TestData.Usings}

namespace ConsoleApplication1
{{
    [Sundew.DiscriminatedUnions.DiscriminatedUnion]
    public abstract record Result
    {{
        private Result()
        {{ }}

        public sealed record Success : Result;

        public sealed record Warning(string Message) : Result;

        public sealed record Error(int Code) : Result;
    }}
}}";

            var expected = new[]
            {
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.ClassDiscriminatedUnionsMustBeAbstractRule)
                    .WithArguments("ConsoleApplication1.Result")
                    .WithSpan(12, 5, 23, 6),
            };
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}