// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchShouldNotHaveDefaultCaseAnalyzerTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test.SwitchExpression
{
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sundew.DiscriminatedUnions.Analyzer;
    using VerifyCS = Sundew.DiscriminatedUnions.Test.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.SundewDiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionSwitchWarningSuppressor>;

    [TestClass]
    public class SwitchShouldNotHaveDefaultCaseAnalyzerTests
    {
        [TestMethod]
        public async Task Given_SwitchExpression_When_DefaultCaseIsHandled_Then_SwitchShouldNotHaveDefaultCaseIsReported()
        {
            var test = $@"#nullable enable
{TestData.Usings}

namespace ConsoleApplication1
{{
    public class DiscriminatedUnionSymbolAnalyzerTests
    {{
        public bool Switch(Result result)
        {{
            return result switch
                {{
                    Result.Success => true,
                    Result.Warning warning => true,
                    Result.Error error => false,
                    _ => false,
                }};
        }}
    }}
{TestData.ValidResultDiscriminatedUnion}
}}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchShouldNotHaveDefaultCaseRule)
                    .WithArguments(TestData.ConsoleApplication1Result)
                    .WithSpan(22, 21, 22, 31));
        }
    }
}
