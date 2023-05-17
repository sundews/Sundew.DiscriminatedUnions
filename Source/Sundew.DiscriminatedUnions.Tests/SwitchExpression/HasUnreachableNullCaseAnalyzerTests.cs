// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HasUnreachableNullCaseAnalyzerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test.SwitchExpression;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Test.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionSwitchWarningSuppressor>;

[TestClass]
public class HasUnreachableNullCaseAnalyzerTests
{
    [TestMethod]
    public async Task Given_SwitchExpressionInEnabledNullableContext_When_AllCasesAndNullAreHandled_Then_NullCaseShouldNotBeHandledIsReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public bool Switch<T>(Option<T> option)
        where T : notnull
    {{
        return option switch
        {{
            Option<T>.Some some => true,
            Option<T>.None => false,
            null => false,
        }};
    }}
}}
{TestData.ValidGenericOptionUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchHasUnreachableNullCaseRule)
                .WithSpan(23, 13, 23, 26));
    }

    [TestMethod]
    public async Task Given_SwitchExpressionInEnabledNullableContext_When_ValueIsNotNullAndAllCasesAndNullCaseAreHandled_Then_HasUnreachableNullCaseIsReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}

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

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchHasUnreachableNullCaseRule)
                .WithSpan(22, 17, 22, 27));
    }

    [TestMethod]
    [Ignore]
    public async Task Given_SwitchExpressionInEnabledNullableContext_When_ValueComeFromAMethodAndIsNotNullAndAllCasesAndNullCaseAreHandled_Then_HasUnreachableNullCaseIsReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public int Switch()
    {{
        var option = Compute(""test"");
        return option switch
            {{
                Option<int>.Some some => some.Value,
                Option<int>.None => 0,
                null => -1,
            }};
    }}

    private static Option<int> Compute(string test)
    {{
        if (test == ""test"")
        {{
            return new Option<int>.Some(45);
        }}

        return new Option<int>.None();
    }}
}}
{TestData.ValidGenericOptionUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchHasUnreachableNullCaseRule)
                .WithSpan(18, 24, 23, 22));
    }
}