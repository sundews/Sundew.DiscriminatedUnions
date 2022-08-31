// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PopulateFactoryMethodsCodeFixerTests.cs" company="Hukano">
// Copyright (c) Hukano. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Test;

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Test.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.PopulateUnionFactoryMethodsMarkerAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionSwitchWarningSuppressor>;

[TestClass]
public class PopulateFactoryMethodsCodeFixerTests
{
    [TestMethod]
    public async Task
       Given_DiscriminatedUnionWithUnnestedCases_When_ValueAndSubtractExpressionCaseveHasNoFactoryMethod_Then_FactoryMethodsAreImplemented()
    {
        var test = $@"{TestData.Usings}

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
            VerifyCS.Diagnostic(PopulateUnionFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsRule)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("Unions.Expression")
                .WithSpan(14, 24, 14, 34),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, true);
    }

    [TestMethod]
    public async Task
        Given_DiscriminatedUnionWithUnnestedCases_When_AdditionAndSubtractExpressionCasesHaveNoFactoryMethod_Then_FactoryMethodsAreImplemented()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Expression
{{
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
    public static Expression ValueExpression(int value) => new ValueExpression(value);

    public static Expression AdditionExpression(Expression lhs, Expression rhs) => new AdditionExpression(lhs, rhs);

    public static Expression SubtractionExpression(Expression lhs, Expression rhs) => new SubtractionExpression(lhs, rhs);
}}

public sealed record AdditionExpression(Expression Lhs, Expression Rhs) : Expression;

public sealed record SubtractionExpression(Expression Lhs, Expression Rhs) : Expression;

public sealed record ValueExpression(int Value) : Expression;
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(PopulateUnionFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsRule)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("Unions.Expression")
                .WithSpan(14, 24, 14, 34),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, true);
    }

    [TestMethod]
    public async Task
        Given_MultiDiscriminatedUnionWithUnnestedCases_When_MultipleCasesHaveNoFactoryMethod_Then_FactoryMethodsAreImplemented()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public interface IAssociativeExpression
{{
    public static IAssociativeExpression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    public static IAssociativeExpression MultiplicationExpression(Expression lhs, Expression rhs) => new MultiplicationExpression(lhs, rhs);
}}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record ArithmeticExpression : Expression
{{
    public abstract Expression Lhs {{ get; init; }}

    public abstract Expression Rhs {{ get; init; }}

    public new static ArithmeticExpression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    public new static ArithmeticExpression SubtractExpression(Expression lhs, Expression rhs) => new SubtractExpression(lhs, rhs);

    public new static ArithmeticExpression MultiplicationExpression(Expression lhs, Expression rhs) => new MultiplicationExpression(lhs, rhs);
}}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Expression
{{
    public static Expression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    public static Expression SubtractExpression(Expression lhs, Expression rhs) => new SubtractExpression(lhs, rhs);
}}

public sealed record AddExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression, IAssociativeExpression;

public sealed record MultiplicationExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression, IAssociativeExpression;

public sealed record SubtractExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression;

public sealed record ValueExpression(int Value) : Expression;
";

        var fixtest = $@"{TestData.Usings}

namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public interface IAssociativeExpression
{{
    public static IAssociativeExpression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    public static IAssociativeExpression MultiplicationExpression(Expression lhs, Expression rhs) => new MultiplicationExpression(lhs, rhs);
}}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record ArithmeticExpression : Expression
{{
    public abstract Expression Lhs {{ get; init; }}

    public abstract Expression Rhs {{ get; init; }}

    public new static ArithmeticExpression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    public new static ArithmeticExpression SubtractExpression(Expression lhs, Expression rhs) => new SubtractExpression(lhs, rhs);

    public new static ArithmeticExpression MultiplicationExpression(Expression lhs, Expression rhs) => new MultiplicationExpression(lhs, rhs);
}}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Expression
{{
    public static Expression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    public static Expression SubtractExpression(Expression lhs, Expression rhs) => new SubtractExpression(lhs, rhs);

    public static Expression MultiplicationExpression(Expression lhs, Expression rhs) => new MultiplicationExpression(lhs, rhs);

    public static Expression ValueExpression(int value) => new ValueExpression(value);
}}

public sealed record AddExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression, IAssociativeExpression;

public sealed record MultiplicationExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression, IAssociativeExpression;

public sealed record SubtractExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression;

public sealed record ValueExpression(int Value) : Expression;
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(PopulateUnionFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsRule)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("Unions.IAssociativeExpression")
                .WithSpan(14, 18, 14, 40),
            VerifyCS.Diagnostic(PopulateUnionFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsRule)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("Unions.Expression")
                .WithSpan(36, 24, 36, 34),
            VerifyCS.Diagnostic(PopulateUnionFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsRule)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("Unions.ArithmeticExpression")
                .WithSpan(22, 24, 22, 44),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, true, 1);
    }

    [TestMethod]
    public async Task Given_MultiDiscriminatedUnionWithUnnestedCases_When_MultipleCasesInDerivedTypeHaveNoFactoryMethod_Then_FactoryMethodsAreImplemented()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public interface IAssociativeExpression
{{
    public static IAssociativeExpression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    public static IAssociativeExpression MultiplicationExpression(Expression lhs, Expression rhs) => new MultiplicationExpression(lhs, rhs);
}}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Expression
{{
    public static Expression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    public static Expression SubtractExpression(Expression lhs, Expression rhs) => new SubtractExpression(lhs, rhs);

    public static Expression MultiplicationExpression(Expression lhs, Expression rhs) => new MultiplicationExpression(lhs, rhs);

    public static Expression ValueExpression(int value) => new ValueExpression(value);
}}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record ArithmeticExpression : Expression
{{
    public abstract Expression Lhs {{ get; init; }}

    public abstract Expression Rhs {{ get; init; }}

    public new static ArithmeticExpression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);
}}

public sealed record AddExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression, IAssociativeExpression;

public sealed record SubtractExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression;

public sealed record MultiplicationExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression, IAssociativeExpression;

public sealed record ValueExpression(int Value) : Expression;
";

        var fixtest = $@"{TestData.Usings}

namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public interface IAssociativeExpression
{{
    public static IAssociativeExpression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    public static IAssociativeExpression MultiplicationExpression(Expression lhs, Expression rhs) => new MultiplicationExpression(lhs, rhs);
}}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Expression
{{
    public static Expression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    public static Expression SubtractExpression(Expression lhs, Expression rhs) => new SubtractExpression(lhs, rhs);

    public static Expression MultiplicationExpression(Expression lhs, Expression rhs) => new MultiplicationExpression(lhs, rhs);

    public static Expression ValueExpression(int value) => new ValueExpression(value);
}}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record ArithmeticExpression : Expression
{{
    public abstract Expression Lhs {{ get; init; }}

    public abstract Expression Rhs {{ get; init; }}

    public new static ArithmeticExpression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    public new static ArithmeticExpression SubtractExpression(Expression lhs, Expression rhs) => new SubtractExpression(lhs, rhs);

    public new static ArithmeticExpression MultiplicationExpression(Expression lhs, Expression rhs) => new MultiplicationExpression(lhs, rhs);
}}

public sealed record AddExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression, IAssociativeExpression;

public sealed record SubtractExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression;

public sealed record MultiplicationExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression, IAssociativeExpression;

public sealed record ValueExpression(int Value) : Expression;
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(PopulateUnionFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsRule)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("Unions.IAssociativeExpression")
                .WithSpan(14, 18, 14, 40),
            VerifyCS.Diagnostic(PopulateUnionFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsRule)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("Unions.Expression")
                .WithSpan(22, 24, 22, 34),
            VerifyCS.Diagnostic(PopulateUnionFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsRule)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("Unions.ArithmeticExpression")
                .WithSpan(34, 24, 34, 44),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, true, 1);
    }

    [TestMethod]
    public async Task
        Given_DiscriminatedUnionWithUnnestedCases_When_UnionIsGenericAndMultipleHasNoFactoryMethod_Then_FactoryMethodsAreImplemented()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record SingleOrMultiple<TItem>
{{
    public static SingleOrMultiple<TItem> Single(TItem item) => new Single<TItem>(item);
}}

public sealed record Single<TItem>(TItem Item) : SingleOrMultiple<TItem>;

public sealed record Multiple<TItem>(IReadOnlyList<TItem> Items) : SingleOrMultiple<TItem>;
";

        var fixtest = $@"{TestData.Usings}

namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record SingleOrMultiple<TItem>
{{
    public static SingleOrMultiple<TItem> Single(TItem item) => new Single<TItem>(item);

    public static SingleOrMultiple<TItem> Multiple(IReadOnlyList<TItem> items) => new Multiple<TItem>(items);
}}

public sealed record Single<TItem>(TItem Item) : SingleOrMultiple<TItem>;

public sealed record Multiple<TItem>(IReadOnlyList<TItem> Items) : SingleOrMultiple<TItem>;
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(PopulateUnionFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsRule)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("Unions.SingleOrMultiple<TItem>")
                .WithSpan(14, 24, 14, 40),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, true);
    }
}