// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnnestedCasesMustHaveFactoryMethodCodeFixTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test
{
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sundew.DiscriminatedUnions.Analyzer;
    using VerifyCS = Sundew.DiscriminatedUnions.Test.CSharpCodeFixVerifier<
        Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionsAnalyzer,
        Sundew.DiscriminatedUnions.CodeFixes.SundewDiscriminatedUnionsCodeFixProvider,
        Sundew.DiscriminatedUnions.Analyzer.SundewDiscriminatedUnionSwitchWarningSuppressor>;

    [TestClass]
    public class UnnestedCasesMustHaveFactoryMethodCodeFixTests
    {
        [TestMethod]
        public async Task
            Given_DiscriminatedUnionWithUnnestedCases_When_ValueExpressionCaseHasNoFactoryMethod_Then_FactoryMethodIsImplemented()
        {
            var test = $@"{TestData.Usings}

namespace ConsoleApplication1
{{
    [Sundew.DiscriminatedUnions.DiscriminatedUnion]
    public abstract record Expression
    {{
        private protected Expression()
        {{ }}

        public static Expression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

        public static Expression SubtractExpression(Expression lhs, Expression rhs) => new SubtractExpression(lhs, rhs);
    }}

    public sealed record AddExpression(Expression Lhs, Expression Rhs) : Expression;

    public sealed record SubtractExpression(Expression Lhs, Expression Rhs) : Expression;

    public sealed record ValueExpression(int Value) : Expression;
}}";

            var fixtest = $@"{TestData.Usings}

namespace ConsoleApplication1
{{
    [Sundew.DiscriminatedUnions.DiscriminatedUnion]
    public abstract record Expression
    {{
        private protected Expression()
        {{ }}

        public static Expression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

        public static Expression SubtractExpression(Expression lhs, Expression rhs) => new SubtractExpression(lhs, rhs);

        public static Expression ValueExpression(int value) => new ValueExpression(value);
    }}

    public sealed record AddExpression(Expression Lhs, Expression Rhs) : Expression;

    public sealed record SubtractExpression(Expression Lhs, Expression Rhs) : Expression;

    public sealed record ValueExpression(int Value) : Expression;
}}";

            var expected = new[]
            {
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.UnnestedCasesShouldHaveFactoryMethodRule)
                    .WithArguments("ConsoleApplication1.ValueExpression", "ConsoleApplication1.Expression")
                    .WithSpan(12, 5, 21, 6),
            };
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }

        [TestMethod]
        public async Task
            Given_DiscriminatedUnionWithUnnestedCases_When_SubtractExpressionCaseHasNoFactoryMethod_Then_FactoryMethodIsImplemented()
        {
            var test = $@"{TestData.Usings}

namespace ConsoleApplication1
{{
    [Sundew.DiscriminatedUnions.DiscriminatedUnion]
    public abstract record Expression
    {{
        private protected Expression()
        {{ }}

        public static Expression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

        public static Expression ValueExpression(int value) => new ValueExpression(value);
    }}

    public sealed record AddExpression(Expression Lhs, Expression Rhs) : Expression;

    public sealed record SubtractExpression(Expression Lhs, Expression Rhs) : Expression;

    public sealed record ValueExpression(int Value) : Expression;
}}";

            var fixtest = $@"{TestData.Usings}

namespace ConsoleApplication1
{{
    [Sundew.DiscriminatedUnions.DiscriminatedUnion]
    public abstract record Expression
    {{
        private protected Expression()
        {{ }}

        public static Expression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

        public static Expression ValueExpression(int value) => new ValueExpression(value);

        public static Expression SubtractExpression(Expression lhs, Expression rhs) => new SubtractExpression(lhs, rhs);
    }}

    public sealed record AddExpression(Expression Lhs, Expression Rhs) : Expression;

    public sealed record SubtractExpression(Expression Lhs, Expression Rhs) : Expression;

    public sealed record ValueExpression(int Value) : Expression;
}}";

            var expected = new[]
            {
                VerifyCS.Diagnostic(SundewDiscriminatedUnionsAnalyzer.UnnestedCasesShouldHaveFactoryMethodRule)
                    .WithArguments("ConsoleApplication1.SubtractExpression", "ConsoleApplication1.Expression")
                    .WithSpan(12, 5, 21, 6),
            };
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}