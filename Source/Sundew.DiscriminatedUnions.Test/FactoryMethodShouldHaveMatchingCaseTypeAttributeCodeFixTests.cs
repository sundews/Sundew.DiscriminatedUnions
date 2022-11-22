// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryMethodShouldHaveMatchingCaseTypeAttributeCodeFixTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Test.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionSwitchWarningSuppressor>;

[TestClass]
public class FactoryMethodShouldHaveMatchingCaseTypeAttributeCodeFixTests
{
    [TestMethod]
    public async Task Given_FactoryMethod_When_NoCaseTypeAttributeIsPresent_Then_CaseTypeIsInserted()
    {
        var test = $@"#nullable enable
{TestData.Usings}
namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal interface Expression
{{
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AdditionExpression))]
    public static Expression AdditionExpression(Expression lhs, Expression rhs) => new AdditionExpression(lhs, rhs);

    public static Expression SubtractionExpression(Expression lhs, Expression rhs) => new SubtractionExpression(lhs, rhs);
}}

internal sealed record AdditionExpression(Expression Lhs, Expression Rhs) : Expression;

internal sealed record SubtractionExpression(Expression Lhs, Expression Rhs) : Expression;
";

        var fixtest = $@"#nullable enable
{TestData.Usings}
namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal interface Expression
{{
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AdditionExpression))]
    public static Expression AdditionExpression(Expression lhs, Expression rhs) => new AdditionExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(SubtractionExpression))]
    public static Expression SubtractionExpression(Expression lhs, Expression rhs) => new SubtractionExpression(lhs, rhs);
}}

internal sealed record AdditionExpression(Expression Lhs, Expression Rhs) : Expression;

internal sealed record SubtractionExpression(Expression Lhs, Expression Rhs) : Expression;
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.FactoryMethodShouldHaveMatchingCaseTypeAttributeRule)
                .WithArguments("Unions.Expression.SubtractionExpression(Unions.Expression, Unions.Expression)", "Unions.SubtractionExpression")
                .WithSpan(19, 5, 19, 123),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
    }

    [TestMethod]
    public async Task Given_FactoryMethod_When_WrongCaseTypeAttributeIsPresent_Then_CorrectCaseIsInserted()
    {
        var test = $@"#nullable enable
{TestData.Usings}
namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal interface Expression
{{
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AdditionExpression))]
    public static Expression AdditionExpression(Expression lhs, Expression rhs) => new AdditionExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AdditionExpression))]
    public static Expression SubtractionExpression(Expression lhs, Expression rhs) => new SubtractionExpression(lhs, rhs);
}}

internal sealed record AdditionExpression(Expression Lhs, Expression Rhs) : Expression;

internal sealed record SubtractionExpression(Expression Lhs, Expression Rhs) : Expression;
";

        var fixtest = $@"#nullable enable
{TestData.Usings}
namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal interface Expression
{{
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AdditionExpression))]
    public static Expression AdditionExpression(Expression lhs, Expression rhs) => new AdditionExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(SubtractionExpression))]
    public static Expression SubtractionExpression(Expression lhs, Expression rhs) => new SubtractionExpression(lhs, rhs);
}}

internal sealed record AdditionExpression(Expression Lhs, Expression Rhs) : Expression;

internal sealed record SubtractionExpression(Expression Lhs, Expression Rhs) : Expression;
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.FactoryMethodShouldHaveMatchingCaseTypeAttributeRule)
                .WithArguments("Unions.Expression.SubtractionExpression(Unions.Expression, Unions.Expression)", "Unions.SubtractionExpression")
                .WithSpan(19, 5, 20, 123)
                .WithSpan(19, 6, 19, 78),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
    }
}