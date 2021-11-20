// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WithSubUnionsAllCasesNotHandledAnalyzerTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test.SwitchExpression;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Test.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.SundewDiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionSwitchWarningSuppressor>;

[TestClass]
public class WithSubUnionsAllCasesNotHandledAnalyzerTests
{
    [TestMethod]
    public async Task Given_SwitchExpression_When_DiscriminatedUnionHasSubUnionAndAllCasesAreNotHandled_Then_AllCasesNotHandledIsReported()
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
                    SubtractExpression subtractExpression => Evaluate(subtractExpression.Lhs) - Evaluate(subtractExpression.Rhs),
                }};
        }}
    }}

    {TestData.ValidDiscriminatedUnionWithSubUnions}
}}";
        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'ValueExpression'", Resources.Case, "ConsoleApplication1.Expression", Resources.Is)
                .WithSpan(16, 20, 20, 18));
    }

    [TestMethod]
    public async Task Given_SwitchExpression_When_DiscriminatedUnionIsSubUnionAndAllCasesAreNotHandled_Then_AllCasesNotHandledIsReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}
namespace ConsoleApplication1
{{
    public class DiscriminatedUnionSymbolAnalyzerTests
    {{   
        public string GetOperator(ArithmeticExpression expression)
        {{
            return expression switch
                {{
                    AddExpression addExpression => ""+"",
                }};
        }}
    }}

    {TestData.ValidDiscriminatedUnionWithSubUnions}
}}";
        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'SubtractExpression'", Resources.Case, "ConsoleApplication1.ArithmeticExpression", Resources.Is)
                .WithSpan(16, 20, 19, 18));
    }
}