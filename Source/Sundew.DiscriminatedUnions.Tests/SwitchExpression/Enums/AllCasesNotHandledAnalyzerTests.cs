﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllCasesNotHandledAnalyzerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Tests.SwitchExpression.Enums;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Analyzer;
using Sundew.DiscriminatedUnions.Tests.Verifiers;
using VerifyCS = Sundew.DiscriminatedUnions.Tests.Verifiers.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionSwitchWarningSuppressor>;

[TestClass]
public class AllCasesNotHandledAnalyzerTests
{
    [TestMethod]
    public async Task
        Given_SwitchExpression_When_DefaultCaseIsHandledAndNotAllCasesAreHandled_Then_AllCasesNotHandledAndSwitchShouldNotHaveDefaultCaseAreReported()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public bool Switch(State state)
    {{
        return state switch
            {{
                State.None => true,
                _ => false,
            }};
    }}
}}
{TestData.ValidEnumUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'On', 'Off'", Resources.Cases, TestData.UnionsState, Resources.Are)
                .WithSpan(17, 16, 21, 14),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchShouldNotHaveDefaultCaseRule)
                .WithArguments(TestData.UnionsState)
                .WithSpan(20, 17, 20, 27));
    }

    [TestMethod]
    public async Task Given_SwitchExpression_When_MultipleCasesAreMissing_Then_AllCasesNotHandledIsReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{
    public bool Switch(State state)
    {{
        return state switch
            {{
                State.On => true,
            }};
    }}
}}
{TestData.ValidEnumUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'None', 'Off'", Resources.Cases, TestData.UnionsState, Resources.Are)
                .WithSpan(18, 16, 21, 14));
    }

    [TestMethod]
    public async Task Given_SwitchExpression_When_ValueMayBeNullAndNullIsNotHandled_Then_AllCasesNotHandledIsReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{
    public bool Switch(State? state)
    {{
        return state switch
            {{
                State.On => true,
                State.Off => true,
                State.None => false,
            }};
    }}
}}
{TestData.ValidEnumUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'null'", Resources.Case, TestData.UnionsState + '?', Resources.Is)
                .WithSpan(18, 16, 23, 14));
    }
}