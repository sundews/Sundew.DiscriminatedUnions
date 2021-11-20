// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchShouldThrowInDefaultCaseAnalyzerTests.cs" company="Hukano">
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
public class SwitchShouldThrowInDefaultCaseAnalyzerTests
{
    [TestMethod]
    public async Task Given_SwitchStatement_When_AllCasesHandledButDefaultCaseIsIncorrect_Then_SwitchShouldThrowInDefaultCaseIsReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace ConsoleApplication1
{{
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
{TestData.ValidResultDiscriminatedUnion}
}}";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchShouldThrowInDefaultCaseRule)
                .WithArguments(TestData.ConsoleApplication1Result)
                .WithSpan(27, 17, 28, 34));
    }

    /*
    [TestMethod]
    public async Task Given_SwitchStatement_When_DefaultCaseIsHandled_Then_SwitchShouldThrowInDefaultCaseIsReported()
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
                .WithSpan(25, 17, 26, 27));
    }*/

    [TestMethod]
    public async Task Given_SwitchStatement_When_AllCasesReturnAndDefaultCaseIsHandled_Then_SwitchShouldThrowInDefaultCaseIsReported()
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
{TestData.ValidResultDiscriminatedUnion}
}}";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchShouldThrowInDefaultCaseRule)
                .WithArguments(TestData.ConsoleApplication1Result)
                .WithSpan(25, 17, 26, 27));
    }

    [TestMethod]
    public async Task Given_SwitchStatement_When_ValueMayBeNullAndAllCasesAreHandledAndDefaultCaseDoesNotThrow_Then_SwitchShouldThrowInDefaultCaseIsReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace ConsoleApplication1
{{
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
{TestData.ValidResultDiscriminatedUnion}
}}";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchShouldThrowInDefaultCaseRule)
                .WithArguments(TestData.ConsoleApplication1Result)
                .WithSpan(29, 17, 30, 34));
    }
}