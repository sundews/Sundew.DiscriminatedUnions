// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoDiagnosticsAnalyzerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test.SwitchExpression;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = Sundew.DiscriminatedUnions.Test.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionSwitchWarningSuppressor>;

[TestClass]
public class NoDiagnosticsAnalyzerTests
{
    [TestMethod]
    public async Task Given_NoDiscriminatedUnionSwitch_Then_NoDiagnosticsAreReported()
    {
        var test = string.Empty;

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task Given_SwitchExpression_When_ExactlyAllCasesAreHandled_Then_NoDiagnosticsAreReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public bool Switch(Result result)
    {{
        return result switch
            {{
                Result.Success success => true,
                Result.Warning {{ Message: ""Tough warning"" }} warning => false,
                Result.Warning warning => true,
                Result.Error error => false,
            }};
    }}
}}
{TestData.ValidResultUnion}
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
    public bool Switch(Result result)
    {{
        return result switch
            {{
                Result.Success success => true,
                Result.Warning {{ Message: ""Tough warning"" }} warning => false,
                Result.Warning warning => true,
                Result.Error error => false,
                null => false,
            }};
    }}
}}
{TestData.ValidResultUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task Given_GenericSwitchExpressionInDisableNullableContext_When_ValueMayBeNullAndAllCasesAndNullCaseAreHandled_Then_NoDiagnosticsAreReported()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public int Switch(Option<int> option)
    {{
        return option switch
            {{
                Option<int>.Some some => some.Value,
                Option<int>.None => 0,
                null => -1,
            }};
    }}
}}
{TestData.ValidGenericOptionUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}