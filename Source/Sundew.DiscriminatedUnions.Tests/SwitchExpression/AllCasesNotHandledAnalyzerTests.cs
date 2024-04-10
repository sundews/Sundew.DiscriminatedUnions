// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllCasesNotHandledAnalyzerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Tests.SwitchExpression;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Analyzer;
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
    public bool Switch(Result result)
    {{
        return result switch
            {{
                Result.Success => true,
                _ => false,
            }};
    }}
}}
{TestData.ValidResultUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Warning', 'Error', 'null'", Resources.Cases, TestData.UnionsResult, Resources.Are)
                .WithSpan(17, 16, 21, 14),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchShouldNotHaveDefaultCaseRule)
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
{TestData.ValidResultUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
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
{TestData.ValidResultUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'null'", Resources.Case, TestData.UnionsResult + '?', Resources.Is)
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
{TestData.ValidGenericOptionUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'null'", Resources.Case, TestData.UnionsOptionInt + '?', Resources.Is)
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
{TestData.ValidGenericOptionUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
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
{TestData.ValidMultiUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'SubtractionExpression', 'ValueExpression'", Resources.Cases, "Unions.Expression", Resources.Are)
                .WithSpan(18, 16, 21, 10));
    }

    [TestMethod]
    public async Task Given_GenericSwitchExpressionInEnabledNullableContext_When_MultipleCasesAreMissingForUnnestedCases_Then_AllCasesNotHandledIsReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record SingleOrMultiple<TItem>
{{
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(Single<>))]
    public static SingleOrMultiple<TItem> Single(TItem item) => new Single<TItem>(item);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(Multiple<>))]
    public static SingleOrMultiple<TItem> Multiple(IReadOnlyList<TItem> item) => new Multiple<TItem>(item);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(Empty<>))]
    public static SingleOrMultiple<TItem> Empty() => new Empty<TItem>();
}}

public sealed record Single<TItem>(TItem Item) : SingleOrMultiple<TItem>;

public sealed record Multiple<TItem>(IReadOnlyList<TItem> Items) : SingleOrMultiple<TItem>;

public sealed record Empty<TItem>() : SingleOrMultiple<TItem>;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public string Evaluate<TItem>(SingleOrMultiple<TItem> singleOrMultiple)
    {{
        return singleOrMultiple switch
        {{
            Single<TItem> single => single.ToString(),
        }};
    }}
}}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Multiple', 'Empty'", Resources.Cases, "Unions.SingleOrMultiple<TItem>", Resources.Are)
                .WithSpan(37, 16, 40, 10));
    }

    [TestMethod]
    public async Task
        Given_SwitchExpression_When_SomeCasesThrowNotImplementedExceptionAndNotAllCasesAreHandled_Then_CaseShouldBeImplementedAndAllCasesNotHandledAreReported()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public int Switch(Result result)
    {{
        return result switch
        {{
            Result.Success => throw new NotImplementedException(),
            Result.Warning => throw new System.NotImplementedException(),
        }};
    }}
}}
{TestData.ValidResultUnion}
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Error', 'null'", Resources.Cases, TestData.UnionsResult, Resources.Are)
                .WithSpan(17, 16, 21, 10),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Success")
                .WithSpan(19, 31, 19, 66),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Warning")
                .WithSpan(20, 31, 20, 73));
    }
}