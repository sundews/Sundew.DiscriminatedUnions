﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchShouldNotHaveDefaultCaseAnalyzerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Tests.SwitchStatement.Enums;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Analyzer;
using Sundew.DiscriminatedUnions.Tests.Verifiers;
using VerifyCS = Sundew.DiscriminatedUnions.Tests.Verifiers.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionSwitchWarningSuppressor>;

[TestClass]
public class SwitchShouldNotHaveDefaultCaseAnalyzerTests
{
    [TestMethod]
    public async Task Given_SwitchStatement_When_DefaultCaseIsHandled_Then_SwitchShouldNotReportAnyDiagnostics()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{
    public void Switch(State state)
    {{
        switch(state)
        {{
            case State.None:
                break;
            case State.Off:
                break;
            case State.On:
                break;
            default:
                break;
        }}
    }}
}}
{TestData.ValidEnumUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task Given_SwitchStatement_When_ValueMayBeNullAndDefaultCaseIsHandled_Then_SwitchNotAllCasesAreHandledIsReported()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public void Switch(State? state)
    {{
        switch(state)
        {{
            case State.None:
                break;
            case State.Off:
                break;
            case State.On:
                break;
            default:
                break;
        }}
    }}
}}
{TestData.ValidEnumUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'null'", Resources.Case, TestData.UnionsState + '?', Resources.Is)
                .WithSpan(17, 9, 27, 10));
    }

    [TestMethod]
    public async Task
        Given_SwitchStatement_When_DefaultCaseIsHandledAndNotAllCasesAreHandled_Then_AllCasesNotHandledIsReported()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public void Switch(State state)
    {{
        switch(state)
        {{
            case State.None:
                break;
            default:
                break;
        }}
    }}
}}
{TestData.ValidEnumUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'On', 'Off'", Resources.Cases, TestData.UnionsState, Resources.Are)
                .WithSpan(17, 9, 23, 10));
    }
}