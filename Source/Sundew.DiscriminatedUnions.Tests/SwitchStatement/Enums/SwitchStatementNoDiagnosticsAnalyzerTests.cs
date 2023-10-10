// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchStatementNoDiagnosticsAnalyzerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Tests.SwitchStatement.Enums;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Tests.Verifiers;
using VerifyCS = Sundew.DiscriminatedUnions.Tests.Verifiers.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionSwitchWarningSuppressor>;

[TestClass]
public class SwitchStatementNoDiagnosticsAnalyzerTests
{
    [TestMethod]
    public async Task Given_NoDiscriminatedUnionSwitch_Then_NoDiagnosticsAreReported()
    {
        var test = $@"{TestData.Usings}
using System.IO;

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public bool Switch(SeekOrigin seekOrigin)
    {{
        switch (seekOrigin)
        {{
            case SeekOrigin.Begin:
                return true;
            case SeekOrigin.Current:
                return false;
            case SeekOrigin.End:
                return true;
            default:
                return false;
        }}
    }}
}}
";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task Given_SwitchStatement_When_ExactlyAllCasesAreHandled_Then_NoDiagnosticsAreReported()
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
        }}
    }}
}}
{TestData.ValidEnumUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task Given_SwitchStatement_When_ValueMayBeNullAndExactlyAllCasesAreHandled_Then_NoDiagnosticsAreReported()
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
            case null:
                break;
        }}
    }}
}}
{TestData.ValidEnumUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}