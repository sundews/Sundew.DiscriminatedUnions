// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryMethodShouldHaveMatchingCaseTypeAttributeAnalyzerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
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
public class FactoryMethodShouldHaveMatchingCaseTypeAttributeAnalyzerTests
{
    [TestMethod]
    public async Task Given_FactoryMethod_When_NoCaseTypeAttributeIsPresent_Then_DiagnosticsAreReported()
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
        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.FactoryMethodShouldHaveMatchingCaseTypeAttributeRule)
                .WithArguments("Unions.Expression.SubtractionExpression(Unions.Expression, Unions.Expression)", "Unions.SubtractionExpression")
                .WithSpan(19, 5, 19, 123));
    }

    [TestMethod]
    public async Task Given_FactoryMethod_When_WrongCaseTypeAttributeIsPresent_Then_DiagnosticsAreReported()
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
        await VerifyCS.VerifyAnalyzerAsync(
            test,
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.FactoryMethodShouldHaveMatchingCaseTypeAttributeRule)
                .WithArguments("Unions.Expression.SubtractionExpression(Unions.Expression, Unions.Expression)", "Unions.SubtractionExpression")
                .WithSpan(19, 5, 20, 123)
                .WithSpan(19, 6, 19, 78));
    }

    [TestMethod]
    public async Task Given_FactoryMethod_When_CaseTypeAttributeIsPresent_Then_NoDiagnosticsAreReported()
    {
        var test = $@"#nullable enable
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
        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task Given_ConditionalFactoryMethod_Then_NoDiagnosticsAreReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}
namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal interface Expression
{{
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AdditionExpression))]
    public static Expression AdditionExpression(Expression lhs, Expression rhs) => new AdditionExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(SubtractionExpression))]
    public static Expression SubtractionExpression(Expression lhs, Expression rhs) => new SubtractionExpression(lhs, rhs);

    public static Expression From(Expression lhs, Expression rhs, bool isAddition)
    {{
        if (isAddition)
        {{
            return new AdditionExpression(lhs, rhs);
        }}

        return new SubtractionExpression(lhs, rhs);
    }}
}}

internal sealed record AdditionExpression(Expression Lhs, Expression Rhs) : Expression;

internal sealed record SubtractionExpression(Expression Lhs, Expression Rhs) : Expression;
";
        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task Given_DifferentlyNamedFactoryMethod_Then_NoDiagnosticsAreReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}
namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
internal interface Expression
{{
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AdditionExpression))]
    public static Expression AdditionExpression(Expression lhs, Expression rhs) => new AdditionExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(SubtractionExpression))]
    public static Expression Subtraction(Expression lhs, Expression rhs) => new SubtractionExpression(lhs, rhs);
}}

internal sealed record AdditionExpression(Expression Lhs, Expression Rhs) : Expression;

internal sealed record SubtractionExpression(Expression Lhs, Expression Rhs) : Expression;
";
        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [TestMethod]
    public async Task Given_PropertyFactoryMethod_Then_NoDiagnosticsAreReported()
    {
        var test = $@"#nullable enable
{TestData.Usings}
namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Scope
{{
    [CaseType(typeof(AutoScope))]
    public static Scope Auto {{ get; }} = new AutoScope();

    [Sundew.DiscriminatedUnions.CaseType(typeof(SingleInstancePerFuncResultScope))]
    public static Scope SingleInstancePerFuncResult(string method) => new SingleInstancePerFuncResultScope(method);

    internal sealed record AutoScope : Scope;

    internal sealed record SingleInstancePerFuncResultScope(string Method) : Scope;
}}
";
        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}