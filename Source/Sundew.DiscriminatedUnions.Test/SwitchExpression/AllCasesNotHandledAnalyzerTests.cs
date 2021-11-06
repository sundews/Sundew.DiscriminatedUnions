// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllCasesNotHandledAnalyzerTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test.SwitchExpression
{
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sundew.DiscriminatedUnions.Analyzer;
    using VerifyCS = Sundew.DiscriminatedUnions.Test.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.SundewDiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionSwitchWarningSuppressor>;

    [TestClass]
    public class AllCasesNotHandledAnalyzerTests
    {
        [TestMethod]
        public async Task
            Given_SwitchExpression_When_DefaultCaseIsHandledAndNotAllCasesAreHandled_Then_AllCasesNotHandledAndSwitchShouldNotHaveDefaultCaseAreReported()
        {
            var test = $@"{TestData.Usings}

namespace ConsoleApplication1
{{
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
}}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                    .WithArguments("'Warning', 'Error', 'null'", Resources.Cases, TestData.ConsoleApplication1Result, Resources.Are)
                    .WithSpan(16, 20, 20, 18),
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchShouldNotHaveDefaultCaseRule)
                    .WithArguments(TestData.ConsoleApplication1Result)
                    .WithSpan(19, 21, 19, 31));
        }

        [TestMethod]
        public async Task Given_SwitchExpression_When_MultipleCasesAreMissing_Then_AllCasesNotHandledIsReported()
        {
            var test = $@"#nullable enable
{TestData.Usings}

namespace ConsoleApplication1
{{
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
}}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                    .WithArguments("'Warning', 'Error'", Resources.Cases, TestData.ConsoleApplication1Result, Resources.Are)
                    .WithSpan(17, 20, 20, 18));
        }

        [TestMethod]
        public async Task Given_SwitchExpression_When_ValueMayBeNullAndNullIsNotHandled_Then_AllCasesNotHandledIsReported()
        {
            var test = $@"#nullable enable
{TestData.Usings}

namespace ConsoleApplication1
{{
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
}}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                    .WithArguments("'null'", Resources.Case, TestData.ConsoleApplication1Result, Resources.Is)
                    .WithSpan(17, 20, 23, 18));
        }

        [TestMethod]
        public async Task Given_GenericSwitchExpressionInEnabledNullableContext_When_ValueMayBeNullAndNullIsNotHandled_Then_AllCasesNotHandledIsReported()
        {
            var test = $@"#nullable enable
{TestData.Usings}

    namespace ConsoleApplication1
    {{
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
    }}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                    .WithArguments("'null'", Resources.Case, TestData.ConsoleApplication1OptionInt, Resources.Is)
                    .WithSpan(17, 24, 21, 22));
        }

        [TestMethod]
        public async Task Given_GenericSwitchExpressionInDisabledNullableContext_When_AllCasesExceptNullAreHandled_Then_AllCasesNotHandledIsReported()
        {
            var test = $@"{TestData.Usings}

namespace ConsoleApplication1
{{
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
}}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                    .WithArguments("'null'", Resources.Case, "ConsoleApplication1.Option<T>", Resources.Is)
                    .WithSpan(17, 20, 21, 14));
        }

        [TestMethod]
        public async Task Given_SwitchExpressionInEnabledNullableContext_When_MultipleCasesAreMissingForUnnestedCases_Then_AllCasesNotHandledIsReported()
        {
            var test = $@"#nullable enable
{TestData.Usings}

namespace ConsoleApplication1
{{
    public class DiscriminatedUnionSymbolAnalyzerTests
    {{   
        public int Evaluate(Expression expression)
        {{
            return expression switch
            {{
                AddExpression addExpression => Evaluate(addExpression.Lhs) + Evaluate(addExpression.Rhs),
            }};
        }}
    }}
{TestData.ValidDiscriminatedUnionWithSubUnions}
}}";

            await VerifyCS.VerifyAnalyzerAsync(
                test,
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                    .WithArguments("'SubtractExpression', 'ValueExpression'", Resources.Cases, "ConsoleApplication1.Expression", Resources.Are)
                    .WithSpan(17, 20, 20, 14));
        }
    }
}
