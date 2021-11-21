﻿// --------------------------------------------------------------------------------------------------------------------
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
    Sundew.DiscriminatedUnions.Analyzer.DimensionalUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DimensionalUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DimensionalUnionSwitchWarningSuppressor>;

[TestClass]
public class UnnestedCasesMustHaveFactoryMethodAnalyzerTests
{
    [TestMethod]
    public async Task
        Given_DiscriminatedUnion_When_CasesAreNotNestedAndNoFactoryMethodIsDeclared_Then_UnnestedCasesShouldHaveFactoryMethodAreReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}
namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Expression
{{
    public static Expression AdditionExpression(Expression lhs, Expression rhs) => new AdditionExpression(lhs, rhs);
}}

public sealed record AdditionExpression(Expression Lhs, Expression Rhs) : Expression;

public sealed record SubtractionExpression(Expression Lhs, Expression Rhs) : Expression;

public sealed record ValueExpression(int Value) : Expression;
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DimensionalUnionsAnalyzer.UnnestedCasesShouldHaveFactoryMethodRule)
                .WithArguments("Unions.SubtractionExpression", "Unions.Expression")
                .WithSpan(13, 1, 17, 2),
            VerifyCS.Diagnostic(DimensionalUnionsAnalyzer.UnnestedCasesShouldHaveFactoryMethodRule)
                .WithArguments("Unions.ValueExpression", "Unions.Expression")
                .WithSpan(13, 1, 17, 2));
    }

    [TestMethod]
    public async Task Given_DiscriminatedUnion_When_UnionIsInterfaceAndCasesAreNotNestedAndNoFactoryMethodIsDeclared_Then_UnnestedCasesShouldHaveFactoryMethodAreReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}
namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal interface Expression
{{
    public static Expression AdditionExpression(Expression lhs, Expression rhs) => new AdditionExpression(lhs, rhs);
}}

internal sealed record AdditionExpression(Expression Lhs, Expression Rhs) : Expression;

internal sealed record SubtractionExpression(Expression Lhs, Expression Rhs) : Expression;

internal sealed record ValueExpression(int Value) : Expression;
";

        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DimensionalUnionsAnalyzer.UnnestedCasesShouldHaveFactoryMethodRule)
                .WithArguments("Unions.SubtractionExpression", "Unions.Expression")
                .WithSpan(13, 1, 17, 2),
            VerifyCS.Diagnostic(DimensionalUnionsAnalyzer.UnnestedCasesShouldHaveFactoryMethodRule)
                .WithArguments("Unions.ValueExpression", "Unions.Expression")
                .WithSpan(13, 1, 17, 2));
    }
}