// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HasUnreachableNullCaseAnalyzerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tests.SwitchExpression;

using System.Threading.Tasks;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Development.Tests.Verifiers.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionSwitchWarningSuppressor>;

public class HasUnreachableNullCaseAnalyzerTests
{
    [Test]
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

    [Test]
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
}