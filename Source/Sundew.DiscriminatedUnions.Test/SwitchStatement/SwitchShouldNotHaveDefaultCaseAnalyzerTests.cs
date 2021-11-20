// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchShouldNotHaveDefaultCaseAnalyzerTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test.SwitchStatement;

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
    public async Task Given_SwitchStatement_When_DefaultCaseIsHandled_Then_SwitchShouldNotHaveDefaultCaseIsReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}

    namespace ConsoleApplication1
    {{
        public class DiscriminatedUnionSymbolAnalyzerTests
        {{
            public void Switch(Result result)
            {{
                switch(result)
                {{
                    case Result.Success:
                        break;
                    case Result.Warning warning:
                        break;
                    case Result.Error error:
                        break;
                    default:
                        break;
                }}
            }}
        }}
{TestData.ValidResultDiscriminatedUnion}
    }}";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchShouldNotHaveDefaultCaseRule)
                .WithArguments(TestData.ConsoleApplication1Result)
                .WithSpan(25, 21, 26, 31));
    }

    [TestMethod]
    public async Task Given_SwitchStatement_When_ValueMayBeNullAndDefaultCaseIsHandled_Then_SwitchShouldNotHaveDefaultCaseIsReported()
    {
        var test = $@"{TestData.Usings}

namespace ConsoleApplication1
{{
    public class DiscriminatedUnionSymbolAnalyzerTests
    {{   
        public void Switch(Result result)
        {{
            switch(result)
            {{
                case Result.Success:
                    break;
                case Result.Warning warning:
                    break;
                case Result.Error error:
                    break;
                default:
                    break;
            }}
        }}
    }}
{TestData.ValidResultDiscriminatedUnion}
}}";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchShouldNotHaveDefaultCaseRule)
                .WithArguments(TestData.ConsoleApplication1Result)
                .WithSpan(24, 17, 25, 27),
            VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'null'", Resources.Case, TestData.ConsoleApplication1Result, Resources.Is)
                .WithSpan(16, 13, 26, 14));
    }

    [TestMethod]
    public async Task
        Given_SwitchStatement_When_DefaultCaseIsHandledAndNotAllCasesAreHandled_Then_AllCasesNotHandledAndSwitchShouldNotHaveDefaultCaseAreReported()
    {
        var test = $@"{TestData.Usings}

namespace ConsoleApplication1
{{
    public class DiscriminatedUnionSymbolAnalyzerTests
    {{   
        public void Switch(Result result)
        {{
            switch(result)
            {{
                case Result.Success:
                    break;
                default:
                    break;
            }}
        }}
    }}
{TestData.ValidResultDiscriminatedUnion}
}}";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Warning', 'Error', 'null'", Resources.Cases, TestData.ConsoleApplication1Result, Resources.Are)
                .WithSpan(16, 13, 22, 14),
            VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchShouldNotHaveDefaultCaseRule)
                .WithArguments(TestData.ConsoleApplication1Result)
                .WithSpan(20, 17, 21, 27));
    }
}