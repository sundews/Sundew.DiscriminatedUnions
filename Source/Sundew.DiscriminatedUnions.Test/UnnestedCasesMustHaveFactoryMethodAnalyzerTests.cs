// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnnestedCasesMustHaveFactoryMethodAnalyzerTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Test.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.SundewDiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionSwitchWarningSuppressor>;

[TestClass]
public class UnnestedCasesMustHaveFactoryMethodAnalyzerTests
{
    [TestMethod]
    public async Task
        Given_DiscriminatedUnion_When_CasesAreNotNestedAndNoFactoryMethodIsDeclared_Then_UnnestedCasesShouldHaveFactoryMethodAreReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}
namespace ConsoleApplication1
{{
    [Sundew.DiscriminatedUnions.DiscriminatedUnion]
    public abstract record Expression
    {{
        private protected Expression()
        {{ }}

        public static Expression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);
    }}

    public sealed record AddExpression(Expression Lhs, Expression Rhs) : Expression;

    public sealed record SubtractExpression(Expression Lhs, Expression Rhs) : Expression;

    public sealed record ValueExpression(int Value) : Expression;
}}";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.UnnestedCasesShouldHaveFactoryMethodRule)
                .WithArguments("ConsoleApplication1.SubtractExpression", "ConsoleApplication1.Expression")
                .WithSpan(12, 5, 19, 6),
            VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.UnnestedCasesShouldHaveFactoryMethodRule)
                .WithArguments("ConsoleApplication1.ValueExpression", "ConsoleApplication1.Expression")
                .WithSpan(12, 5, 19, 6));
    }

    [TestMethod]
    public async Task Given_DiscriminatedUnion_When_UnionIsInterfaceAndCasesAreNotNestedAndNoFactoryMethodIsDeclared_Then_UnnestedCasesShouldHaveFactoryMethodAreReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}
namespace ConsoleApplication1
{{
    [Sundew.DiscriminatedUnions.DiscriminatedUnion]
    internal interface Expression
    {{
        public static Expression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);
    }}

    internal sealed record AddExpression(Expression Lhs, Expression Rhs) : Expression;

    internal sealed record SubtractExpression(Expression Lhs, Expression Rhs) : Expression;

    internal sealed record ValueExpression(int Value) : Expression;
}}";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.UnnestedCasesShouldHaveFactoryMethodRule)
                .WithArguments("ConsoleApplication1.SubtractExpression", "ConsoleApplication1.Expression")
                .WithSpan(12, 5, 16, 6),
            VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.UnnestedCasesShouldHaveFactoryMethodRule)
                .WithArguments("ConsoleApplication1.ValueExpression", "ConsoleApplication1.Expression")
                .WithSpan(12, 5, 16, 6));
    }
}