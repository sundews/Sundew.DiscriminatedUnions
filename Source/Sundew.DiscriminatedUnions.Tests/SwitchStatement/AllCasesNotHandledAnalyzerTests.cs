// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllCasesNotHandledAnalyzerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Tests.SwitchStatement;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Tests.Verifiers.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionSwitchWarningSuppressor>;

[TestClass]
public class AllCasesNotHandledAnalyzerTests
{
    [TestMethod]
    public async Task Given_SwitchStatement_When_MultipleCasesAreMissing_Then_AllCasesNotHandledIsReported()
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
        }}
    }}
}}
{TestData.ValidResultUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Warning', 'Error', 'null'", Resources.Cases, TestData.UnionsResult, Resources.Are)
                .WithSpan(17, 9, 21, 10));
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
{TestData.ValidResultUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Warning', 'Error', 'null'", Resources.Cases, TestData.UnionsResult, Resources.Are)
                .WithSpan(17, 9, 23, 10),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchShouldNotHaveDefaultCaseRule)
                .WithArguments(TestData.UnionsResult)
                .WithSpan(21, 13, 22, 23));
    }

    [TestMethod]
    public async Task
        Given_SwitchStatement_When_SomeCasesThrowNotImplementedExceptionAndNotAllCasesAreHandled_Then_CaseShouldBeImplementedAndAllCasesNotHandledAreReported()
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
                throw new NotImplementedException();
            case Result.Warning:
                throw new System.NotImplementedException();
        }}
    }}
}}
{TestData.ValidResultUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Error', 'null'", Resources.Cases, TestData.UnionsResult, Resources.Are)
                .WithSpan(17, 9, 23, 10),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Success")
                .WithSpan(20, 17, 20, 53),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Warning")
                .WithSpan(22, 17, 22, 60));
    }
}