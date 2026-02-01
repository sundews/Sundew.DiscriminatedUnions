// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnnestedCasesMustHaveFactoryMethodAnalyzerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tests;

using System.Threading.Tasks;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Development.Tests.Verifiers.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionSwitchWarningSuppressor>;

public class UnnestedCasesMustHaveFactoryMethodAnalyzerTests
{
    [Test]
    public async Task Given_Union_When_CasesAreNotNestedAndNoFactoryMethodIsDeclared_Then_UnnestedCasesShouldHaveFactoryMethodAreReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}
namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Expression
{{
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AdditionExpression))]
    public static Expression AdditionExpression(Expression lhs, Expression rhs) => new AdditionExpression(lhs, rhs);
}}

public sealed record AdditionExpression(Expression Lhs, Expression Rhs) : Expression;

public sealed record SubtractionExpression(Expression Lhs, Expression Rhs) : Expression;

public sealed record ValueExpression(int Value) : Expression;
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.UnnestedCasesShouldHaveFactoryMethodRule)
                .WithArguments("Unions.SubtractionExpression", "Unions.Expression")
                .WithSpan(22, 1, 22, 89)
                .WithSpan(13, 1, 18, 2),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.UnnestedCasesShouldHaveFactoryMethodRule)
                .WithArguments("Unions.ValueExpression", "Unions.Expression")
                .WithSpan(24, 1, 24, 62)
                .WithSpan(13, 1, 18, 2));
    }

    [Test]
    public async Task Given_Union_When_UnionIsInterfaceAndCasesAreNotNestedAndNoFactoryMethodIsDeclared_Then_UnnestedCasesShouldHaveFactoryMethodAreReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}
namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal interface Expression
{{
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AdditionExpression))]
    public static Expression AdditionExpression(Expression lhs, Expression rhs) => new AdditionExpression(lhs, rhs);
}}

internal sealed record AdditionExpression(Expression Lhs, Expression Rhs) : Expression;

internal sealed record SubtractionExpression(Expression Lhs, Expression Rhs) : Expression;

internal sealed record ValueExpression(int Value) : Expression;
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.UnnestedCasesShouldHaveFactoryMethodRule)
                .WithArguments("Unions.SubtractionExpression", "Unions.Expression")
                .WithSpan(22, 1, 22, 91)
                .WithSpan(13, 1, 18, 2),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.UnnestedCasesShouldHaveFactoryMethodRule)
                .WithArguments("Unions.ValueExpression", "Unions.Expression")
                .WithSpan(24, 1, 24, 64)
                .WithSpan(13, 1, 18, 2));
    }
}