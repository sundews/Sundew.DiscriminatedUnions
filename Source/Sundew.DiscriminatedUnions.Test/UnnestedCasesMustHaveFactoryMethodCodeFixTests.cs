// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnnestedCasesMustHaveFactoryMethodCodeFixTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Test.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.DimensionalUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DimensionalUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DimensionalUnionSwitchWarningSuppressor>;

[TestClass]
public class UnnestedCasesMustHaveFactoryMethodCodeFixTests
{
    [TestMethod]
    public async Task
        Given_DiscriminatedUnionWithUnnestedCases_When_ValueExpressionCaseHasNoFactoryMethod_Then_FactoryMethodIsImplemented()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Expression
{{
    public static Expression AdditionExpression(Expression lhs, Expression rhs) => new AdditionExpression(lhs, rhs);

    public static Expression SubtractionExpression(Expression lhs, Expression rhs) => new SubtractionExpression(lhs, rhs);
}}

public sealed record AdditionExpression(Expression Lhs, Expression Rhs) : Expression;

public sealed record SubtractionExpression(Expression Lhs, Expression Rhs) : Expression;

public sealed record ValueExpression(int Value) : Expression;
";

        var fixtest = $@"{TestData.Usings}

namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Expression
{{
    public static Expression AdditionExpression(Expression lhs, Expression rhs) => new AdditionExpression(lhs, rhs);

    public static Expression SubtractionExpression(Expression lhs, Expression rhs) => new SubtractionExpression(lhs, rhs);

    public static Expression ValueExpression(int value) => new ValueExpression(value);
}}

public sealed record AdditionExpression(Expression Lhs, Expression Rhs) : Expression;

public sealed record SubtractionExpression(Expression Lhs, Expression Rhs) : Expression;

public sealed record ValueExpression(int Value) : Expression;
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(DimensionalUnionsAnalyzer.UnnestedCasesShouldHaveFactoryMethodRule)
                .WithArguments("Unions.ValueExpression", "Unions.Expression")
                .WithSpan(13, 1, 19, 2),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
    }

    [TestMethod]
    public async Task
        Given_DiscriminatedUnionWithUnnestedCases_When_SubtractExpressionCaseHasNoFactoryMethod_Then_FactoryMethodIsImplemented()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Expression
{{
    public static Expression AdditionExpression(Expression lhs, Expression rhs) => new AdditionExpression(lhs, rhs);

    public static Expression ValueExpression(int value) => new ValueExpression(value);
}}

public sealed record AdditionExpression(Expression Lhs, Expression Rhs) : Expression;

public sealed record SubtractionExpression(Expression Lhs, Expression Rhs) : Expression;

public sealed record ValueExpression(int Value) : Expression;
";

        var fixtest = $@"{TestData.Usings}

namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Expression
{{
    public static Expression AdditionExpression(Expression lhs, Expression rhs) => new AdditionExpression(lhs, rhs);

    public static Expression ValueExpression(int value) => new ValueExpression(value);

    public static Expression SubtractionExpression(Expression lhs, Expression rhs) => new SubtractionExpression(lhs, rhs);
}}

public sealed record AdditionExpression(Expression Lhs, Expression Rhs) : Expression;

public sealed record SubtractionExpression(Expression Lhs, Expression Rhs) : Expression;

public sealed record ValueExpression(int Value) : Expression;
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(DimensionalUnionsAnalyzer.UnnestedCasesShouldHaveFactoryMethodRule)
                .WithArguments("Unions.SubtractionExpression", "Unions.Expression")
                .WithSpan(13, 1, 19, 2),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
    }
}