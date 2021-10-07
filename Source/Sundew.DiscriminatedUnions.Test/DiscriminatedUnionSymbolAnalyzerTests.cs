// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscriminatedUnionSymbolAnalyzerTests.cs" company="Hukano">
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
        Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionsCodeFixProvider,
        Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionSwitchWarningSuppressor>;

    [TestClass]
    public class DiscriminatedUnionSymbolAnalyzerTests
    {
        [TestMethod]
        public async Task Given_DiscriminatedUnion_When_ConstructorIsNotPrivateProtectedAndCaseIsNotSealed_Then_DiscriminatedUnionCanOnlyHavePrivateProtectedConstructorsAndCasesShouldBeSealedAreReported()
        {
            var test = $@"#nullable enable
{TestData.Usings}
    namespace ConsoleApplication1
    {{
        [Sundew.DiscriminatedUnions.DiscriminatedUnion]
        public abstract class Result
        {{
            protected Result()
            {{ }}

            public class Success : Result
            {{
            }}

            public sealed class Warning : Result
            {{
                public Warning(string message)
                {{
                    this.Message = message;
                }}

                public string Message {{ get; private set; }}
            }}
        }}

        public sealed class Error : Result
        {{
            public Error(int code)
            {{
                this.Code = code;
            }}

            public int Code {{ get; private set; }}
        }}
    }}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer
                        .DiscriminatedUnionCanOnlyHavePrivateProtectedConstructorsDiagnosticId)
                    .WithArguments(TestData.ConsoleApplication1Result)
                    .WithSpan(15, 13, 16, 16),
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.CasesShouldBeSealedDiagnosticId)
                    .WithArguments("ConsoleApplication1.Result.Success")
                    .WithSpan(18, 13, 20, 14),
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.CasesShouldBeNestedDiagnosticId)
                    .WithArguments("ConsoleApplication1.Error", TestData.ConsoleApplication1Result)
                    .WithSpan(33, 9, 41, 10));
        }
    }
}