// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchShouldThrowInDefaultCaseAnalyzerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tests.SwitchStatement;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Development.Tests.Verifiers.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionSwitchWarningSuppressor>;

[TestClass]
public class SwitchShouldThrowInDefaultCaseAnalyzerTests
{
    [TestMethod]
    public async Task Given_SwitchStatement_When_AllCasesHandledButDefaultCaseIsIncorrect_Then_SwitchShouldThrowInDefaultCaseIsReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public bool Switch(Result result)
    {{
        switch (result)
        {{
            case Result.Success:
                return true;
            case Result.Warning {{ Message: ""Tough warning"" }} warning:
                return false;
            case Result.Warning warning:
                return true;
            case Result.Error error:
                return false;
            default:
                return false;
        }}
    }}
}}
{TestData.ValidResultUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchShouldThrowInDefaultCaseRule)
                .WithArguments(TestData.UnionsResult)
                .WithSpan(28, 13, 29, 30));
    }

    /*
    [TestMethod]
    public async Task Given_SwitchStatement_When_DefaultCaseIsHandled_Then_SwitchShouldThrowInDefaultCaseIsReported()
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
{TestData.ValidResultUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchShouldNotHaveDefaultCaseRule)
                .WithArguments(TestData.UnionsResult)
                .WithSpan(26, 17, 27, 27));
    }*/

    [TestMethod]
    public async Task Given_SwitchStatement_When_AllCasesReturnAndDefaultCaseIsHandled_Then_SwitchShouldThrowInDefaultCaseIsReported()
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
                return;
            case Result.Warning warning:
                return;
            case Result.Error error:
                return;
            default:
                break;
        }}
    }}
}}
{TestData.ValidResultUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchShouldThrowInDefaultCaseRule)
                .WithArguments(TestData.UnionsResult)
                .WithSpan(26, 13, 27, 23));
    }

    [TestMethod]
    public async Task Given_SwitchStatement_When_ValueMayBeNullAndAllCasesAreHandledAndDefaultCaseDoesNotThrow_Then_SwitchShouldThrowInDefaultCaseIsReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public bool Switch(Result? result)
    {{
        switch (result)
        {{
            case Result.Success:
                return true;
            case Result.Warning {{ Message: ""Tough warning"" }} warning:
                return false;
            case Result.Warning warning:
                return true;
            case Result.Error error:
                return false;
            case null:
                return false;
            default:
                return false;
        }}
    }}
}}
{TestData.ValidResultUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchShouldThrowInDefaultCaseRule)
                .WithArguments(TestData.UnionsResult + '?')
                .WithSpan(30, 13, 31, 30));
    }
}