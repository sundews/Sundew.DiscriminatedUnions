// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoDiagnosticsAnalyzerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tests.SwitchExpression.Enums;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Sundew.DiscriminatedUnions.Development.Tests.Verifiers.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionSwitchWarningSuppressor>;

[TestClass]
public class NoDiagnosticsAnalyzerTests
{
    [TestMethod]
    public async Task Given_SwitchExpression_When_ExactlyAllCasesAreHandled_Then_NoDiagnosticsAreReported()
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
                State.None => true,
                State.On => true,
                State.Off => false,
            }};
    }}
}}
{TestData.ValidEnumUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task Given_SwitchExpression_When_ValueMayBeNullAndExactlyAllCasesAreHandled_Then_NoDiagnosticsAreReported()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public bool Switch(State? state)
    {{
        return state switch
            {{
                State.None => true,
                State.On => true,
                State.Off => false,
                null => false,
            }};
    }}
}}
{TestData.ValidEnumUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}