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
    Sundew.DiscriminatedUnions.Analyzer.DimensionalUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DimensionalUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DimensionalUnionSwitchWarningSuppressor>;

[TestClass]
public class SwitchShouldNotHaveDefaultCaseAnalyzerTests
{
    [TestMethod]
    public async Task Given_SwitchStatement_When_DefaultCaseIsHandled_Then_SwitchShouldNotHaveDefaultCaseIsReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace Unions;

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
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DimensionalUnionsAnalyzer.SwitchShouldNotHaveDefaultCaseRule)
                .WithArguments(TestData.UnionsResult)
                .WithSpan(26, 13, 27, 23));
    }

    [TestMethod]
    public async Task Given_SwitchStatement_When_ValueMayBeNullAndDefaultCaseIsHandled_Then_SwitchShouldNotHaveDefaultCaseIsReported()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

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
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DimensionalUnionsAnalyzer.SwitchShouldNotHaveDefaultCaseRule)
                .WithArguments(TestData.UnionsResult)
                .WithSpan(25, 13, 26, 23),
            VerifyCS.Diagnostic(DimensionalUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'null'", Resources.Case, TestData.UnionsResult, Resources.Is)
                .WithSpan(17, 9, 27, 10));
    }

    [TestMethod]
    public async Task
        Given_SwitchStatement_When_DefaultCaseIsHandledAndNotAllCasesAreHandled_Then_AllCasesNotHandledAndSwitchShouldNotHaveDefaultCaseAreReported()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

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
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DimensionalUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Warning', 'Error', 'null'", Resources.Cases, TestData.UnionsResult, Resources.Are)
                .WithSpan(17, 9, 23, 10),
            VerifyCS.Diagnostic(DimensionalUnionsAnalyzer.SwitchShouldNotHaveDefaultCaseRule)
                .WithArguments(TestData.UnionsResult)
                .WithSpan(21, 13, 22, 23));
    }
}