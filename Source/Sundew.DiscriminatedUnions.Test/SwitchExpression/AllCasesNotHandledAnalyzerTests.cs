// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllCasesNotHandledAnalyzerTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test.SwitchExpression;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Test.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.DimensionalUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DimensionalUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DimensionalUnionSwitchWarningSuppressor>;

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
    public bool Switch(Result result)
    {{
        return result switch
            {{
                Result.Success => true,
                _ => false,
            }};
    }}
}}
{TestData.ValidResultDiscriminatedUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DimensionalUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Warning', 'Error', 'null'", Resources.Cases, TestData.UnionsResult, Resources.Are)
                .WithSpan(17, 16, 21, 14),
            VerifyCS.Diagnostic(DimensionalUnionsAnalyzer.SwitchShouldNotHaveDefaultCaseRule)
                .WithArguments(TestData.UnionsResult)
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
    public bool Switch(Result result)
    {{
        return result switch
            {{
                Result.Success => true,
            }};
    }}
}}
{TestData.ValidResultDiscriminatedUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DimensionalUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Warning', 'Error'", Resources.Cases, TestData.UnionsResult, Resources.Are)
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
    public bool Switch(Result? result)
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
{TestData.ValidResultDiscriminatedUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DimensionalUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'null'", Resources.Case, TestData.UnionsResult, Resources.Is)
                .WithSpan(18, 16, 24, 14));
    }

    [TestMethod]
    public async Task Given_GenericSwitchExpressionInEnabledNullableContext_When_ValueMayBeNullAndNullIsNotHandled_Then_AllCasesNotHandledIsReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public bool Switch(Option<int>? option)
    {{
        return option switch
            {{
                Option<int>.Some => true,
                Option<int>.None => false,
            }};
    }}
}}
{TestData.ValidGenericOptionDiscriminatedUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DimensionalUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'null'", Resources.Case, TestData.UnionsOptionInt, Resources.Is)
                .WithSpan(18, 16, 22, 14));
    }

    [TestMethod]
    public async Task Given_GenericSwitchExpressionInDisabledNullableContext_When_AllCasesExceptNullAreHandled_Then_AllCasesNotHandledIsReported()
    {
        var test = $@"{TestData.Usings}

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
        }};
    }}
}}
{TestData.ValidGenericOptionDiscriminatedUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DimensionalUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'null'", Resources.Case, "Unions.Option<T>", Resources.Is)
                .WithSpan(18, 16, 22, 10));
    }

    [TestMethod]
    public async Task Given_SwitchExpressionInEnabledNullableContext_When_MultipleCasesAreMissingForUnnestedCases_Then_AllCasesNotHandledIsReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public int Evaluate(Expression expression)
    {{
        return expression switch
        {{
            AdditionExpression additionExpression => Evaluate(additionExpression.Lhs) + Evaluate(additionExpression.Rhs),
        }};
    }}
}}
{TestData.ValidDiscriminatedUnionWithSubUnions}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DimensionalUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'SubtractionExpression', 'ValueExpression'", Resources.Cases, "Unions.Expression", Resources.Are)
                .WithSpan(18, 16, 21, 10));
    }
}