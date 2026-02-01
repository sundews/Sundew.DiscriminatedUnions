// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PopulateFactoryMethodsCodeFixerTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tests;

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Development.Tests.Verifiers.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.PopulateFactoryMethodsMarkerAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionSwitchWarningSuppressor>;

public class PopulateFactoryMethodsCodeFixerTests
{
    [Test]
    public async Task
        Given_DiscriminatedUnionWithUnnestedCases_When_ValueAndSubtractExpressionCaseHasNoFactoryMethod_Then_FactoryMethodsAreImplemented()
    {
        var test = $@"{TestData.Usings}

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

        var fixtest = $@"{TestData.Usings}

namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Expression
{{
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AdditionExpression))]
    public static Expression AdditionExpression(Expression lhs, Expression rhs) => new AdditionExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(SubtractionExpression))]
    public static Expression SubtractionExpression(Expression lhs, Expression rhs) => new SubtractionExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(ValueExpression))]
    public static Expression ValueExpression(int value) => new ValueExpression(value);
}}

public sealed record AdditionExpression(Expression Lhs, Expression Rhs) : Expression;

public sealed record SubtractionExpression(Expression Lhs, Expression Rhs) : Expression;

public sealed record ValueExpression(int Value) : Expression;
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(PopulateFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsRule)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("Unions.Expression")
                .WithSpan(14, 24, 14, 34),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, true);
    }

    [Test]
    public async Task Given_DiscriminatedUnionWithUnnestedCases_When_AdditionAndSubtractExpressionCasesHaveNoFactoryMethod_Then_FactoryMethodsAreImplemented()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Expression
{{
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(ValueExpression))]
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
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(ValueExpression))]
    public static Expression ValueExpression(int value) => new ValueExpression(value);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AdditionExpression))]
    public static Expression AdditionExpression(Expression lhs, Expression rhs) => new AdditionExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(SubtractionExpression))]
    public static Expression SubtractionExpression(Expression lhs, Expression rhs) => new SubtractionExpression(lhs, rhs);
}}

public sealed record AdditionExpression(Expression Lhs, Expression Rhs) : Expression;

public sealed record SubtractionExpression(Expression Lhs, Expression Rhs) : Expression;

public sealed record ValueExpression(int Value) : Expression;
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(PopulateFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsRule)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("Unions.Expression")
                .WithSpan(14, 24, 14, 34),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, true);
    }

    [Test]
    public async Task Given_MultiDiscriminatedUnionWithUnnestedCases_When_MultipleCasesHaveNoFactoryMethod_Then_FactoryMethodsAreImplemented()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public interface IAssociativeExpression
{{
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AddExpression))]
    public static IAssociativeExpression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(MultiplicationExpression))]
    public static IAssociativeExpression MultiplicationExpression(Expression lhs, Expression rhs) => new MultiplicationExpression(lhs, rhs);
}}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record ArithmeticExpression : Expression
{{
    public abstract Expression Lhs {{ get; init; }}

    public abstract Expression Rhs {{ get; init; }}

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AddExpression))]
    public new static ArithmeticExpression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(SubtractExpression))]
    public new static ArithmeticExpression SubtractExpression(Expression lhs, Expression rhs) => new SubtractExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(MultiplicationExpression))]
    public new static ArithmeticExpression MultiplicationExpression(Expression lhs, Expression rhs) => new MultiplicationExpression(lhs, rhs);
}}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Expression
{{
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AddExpression))]
    public static Expression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(SubtractExpression))]
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
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AddExpression))]
    public static IAssociativeExpression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(MultiplicationExpression))]
    public static IAssociativeExpression MultiplicationExpression(Expression lhs, Expression rhs) => new MultiplicationExpression(lhs, rhs);
}}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record ArithmeticExpression : Expression
{{
    public abstract Expression Lhs {{ get; init; }}

    public abstract Expression Rhs {{ get; init; }}

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AddExpression))]
    public new static ArithmeticExpression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(SubtractExpression))]
    public new static ArithmeticExpression SubtractExpression(Expression lhs, Expression rhs) => new SubtractExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(MultiplicationExpression))]
    public new static ArithmeticExpression MultiplicationExpression(Expression lhs, Expression rhs) => new MultiplicationExpression(lhs, rhs);
}}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Expression
{{
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AddExpression))]
    public static Expression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(SubtractExpression))]
    public static Expression SubtractExpression(Expression lhs, Expression rhs) => new SubtractExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(MultiplicationExpression))]
    public static Expression MultiplicationExpression(Expression lhs, Expression rhs) => new MultiplicationExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(ValueExpression))]
    public static Expression ValueExpression(int value) => new ValueExpression(value);
}}

public sealed record AddExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression, IAssociativeExpression;

public sealed record MultiplicationExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression, IAssociativeExpression;

public sealed record SubtractExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression;

public sealed record ValueExpression(int Value) : Expression;
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(PopulateFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsRule)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("Unions.IAssociativeExpression")
                .WithSpan(14, 18, 14, 40),
            VerifyCS.Diagnostic(PopulateFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsRule)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("Unions.ArithmeticExpression")
                .WithSpan(24, 24, 24, 44),
            VerifyCS.Diagnostic(PopulateFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsRule)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("Unions.Expression")
                .WithSpan(41, 24, 41, 34),
        };
        var expectedAfter = new[]
        {
            VerifyCS.Diagnostic(PopulateFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsRule)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("Unions.IAssociativeExpression")
                .WithSpan(14, 18, 14, 40),
            VerifyCS.Diagnostic(PopulateFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsRule)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("Unions.ArithmeticExpression")
                .WithSpan(24, 24, 24, 44),
            VerifyCS.Diagnostic(PopulateFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsRule)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("Unions.Expression")
                .WithSpan(41, 24, 41, 34),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, expectedAfter, 1);
    }

    [Test]
    public async Task Given_MultiDiscriminatedUnionWithUnnestedCases_When_UnionIsInterfaceAndFactoryMethodIsMissing_Then_FactoryMethodsAreImplemented()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public interface IAssociativeExpression
{{
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AddExpression))]
    public static IAssociativeExpression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);
}}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record ArithmeticExpression : Expression
{{
    public abstract Expression Lhs {{ get; init; }}

    public abstract Expression Rhs {{ get; init; }}

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AddExpression))]
    public new static ArithmeticExpression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(MultiplicationExpression))]
    public new static ArithmeticExpression MultiplicationExpression(Expression lhs, Expression rhs) => new MultiplicationExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(SubtractExpression))]
    public new static ArithmeticExpression SubtractExpression(Expression lhs, Expression rhs) => new SubtractExpression(lhs, rhs);
}}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Expression
{{
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AddExpression))]
    public static Expression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(MultiplicationExpression))]
    public static Expression MultiplicationExpression(Expression lhs, Expression rhs) => new MultiplicationExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(SubtractExpression))]
    public static Expression SubtractExpression(Expression lhs, Expression rhs) => new SubtractExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(ValueExpression))]
    public static Expression ValueExpression(int value) => new ValueExpression(value);
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
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AddExpression))]
    public static IAssociativeExpression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(MultiplicationExpression))]
    public static IAssociativeExpression MultiplicationExpression(Expression lhs, Expression rhs) => new MultiplicationExpression(lhs, rhs);
}}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record ArithmeticExpression : Expression
{{
    public abstract Expression Lhs {{ get; init; }}

    public abstract Expression Rhs {{ get; init; }}

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AddExpression))]
    public new static ArithmeticExpression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(MultiplicationExpression))]
    public new static ArithmeticExpression MultiplicationExpression(Expression lhs, Expression rhs) => new MultiplicationExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(SubtractExpression))]
    public new static ArithmeticExpression SubtractExpression(Expression lhs, Expression rhs) => new SubtractExpression(lhs, rhs);
}}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Expression
{{
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AddExpression))]
    public static Expression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(MultiplicationExpression))]
    public static Expression MultiplicationExpression(Expression lhs, Expression rhs) => new MultiplicationExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(SubtractExpression))]
    public static Expression SubtractExpression(Expression lhs, Expression rhs) => new SubtractExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(ValueExpression))]
    public static Expression ValueExpression(int value) => new ValueExpression(value);
}}

public sealed record AddExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression, IAssociativeExpression;

public sealed record MultiplicationExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression, IAssociativeExpression;

public sealed record SubtractExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression;

public sealed record ValueExpression(int Value) : Expression;
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(PopulateFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsRule)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("Unions.IAssociativeExpression")
                .WithSpan(14, 18, 14, 40),
            VerifyCS.Diagnostic(PopulateFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsRule)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("Unions.ArithmeticExpression")
                .WithSpan(21, 24, 21, 44),
            VerifyCS.Diagnostic(PopulateFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsRule)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("Unions.Expression")
                .WithSpan(38, 24, 38, 34),
        };
        var expectedAfter = new[]
        {
            VerifyCS.Diagnostic(PopulateFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsRule)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("Unions.IAssociativeExpression")
                .WithSpan(14, 18, 14, 40),
            VerifyCS.Diagnostic(PopulateFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsRule)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("Unions.ArithmeticExpression")
                .WithSpan(24, 24, 24, 44),
            VerifyCS.Diagnostic(PopulateFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsRule)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("Unions.Expression")
                .WithSpan(41, 24, 41, 34),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, expectedAfter, 2);
    }

    [Test]
    public async Task Given_MultiDiscriminatedUnionWithUnnestedCases_When_MultipleCasesInDerivedTypeHaveNoFactoryMethod_Then_FactoryMethodsAreImplemented()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public interface IAssociativeExpression
{{
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AddExpression))]
    public static IAssociativeExpression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(MultiplicationExpression))]
    public static IAssociativeExpression MultiplicationExpression(Expression lhs, Expression rhs) => new MultiplicationExpression(lhs, rhs);
}}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Expression
{{
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AddExpression))]
    public static Expression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(SubtractExpression))]
    public static Expression SubtractExpression(Expression lhs, Expression rhs) => new SubtractExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(MultiplicationExpression))]
    public static Expression MultiplicationExpression(Expression lhs, Expression rhs) => new MultiplicationExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(ValueExpression))]
    public static Expression ValueExpression(int value) => new ValueExpression(value);
}}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record ArithmeticExpression : Expression
{{
    public abstract Expression Lhs {{ get; init; }}

    public abstract Expression Rhs {{ get; init; }}

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AddExpression))]
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
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AddExpression))]
    public static IAssociativeExpression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(MultiplicationExpression))]
    public static IAssociativeExpression MultiplicationExpression(Expression lhs, Expression rhs) => new MultiplicationExpression(lhs, rhs);
}}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record Expression
{{
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AddExpression))]
    public static Expression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(SubtractExpression))]
    public static Expression SubtractExpression(Expression lhs, Expression rhs) => new SubtractExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(MultiplicationExpression))]
    public static Expression MultiplicationExpression(Expression lhs, Expression rhs) => new MultiplicationExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(ValueExpression))]
    public static Expression ValueExpression(int value) => new ValueExpression(value);
}}

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record ArithmeticExpression : Expression
{{
    public abstract Expression Lhs {{ get; init; }}

    public abstract Expression Rhs {{ get; init; }}

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(AddExpression))]
    public new static ArithmeticExpression AddExpression(Expression lhs, Expression rhs) => new AddExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(SubtractExpression))]
    public new static ArithmeticExpression SubtractExpression(Expression lhs, Expression rhs) => new SubtractExpression(lhs, rhs);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(MultiplicationExpression))]
    public new static ArithmeticExpression MultiplicationExpression(Expression lhs, Expression rhs) => new MultiplicationExpression(lhs, rhs);
}}

public sealed record AddExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression, IAssociativeExpression;

public sealed record SubtractExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression;

public sealed record MultiplicationExpression(Expression Lhs, Expression Rhs) : ArithmeticExpression, IAssociativeExpression;

public sealed record ValueExpression(int Value) : Expression;
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(PopulateFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsRule)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("Unions.IAssociativeExpression")
                .WithSpan(14, 18, 14, 40),
            VerifyCS.Diagnostic(PopulateFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsRule)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("Unions.Expression")
                .WithSpan(24, 24, 24, 34),
            VerifyCS.Diagnostic(PopulateFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsRule)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("Unions.ArithmeticExpression")
                .WithSpan(40, 24, 40, 44),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, true, 1);
    }

    [Test]
    public async Task Given_DiscriminatedUnionWithUnnestedCases_When_UnionIsGenericAndMultipleHasNoFactoryMethod_Then_FactoryMethodsAreImplemented()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record SingleOrMultiple<TItem>
{{
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(Single<>))]
    public static SingleOrMultiple<TItem> Single(TItem item) => new Single<TItem>(item);
}}

public sealed record Single<TItem>(TItem Item) : SingleOrMultiple<TItem>;

public sealed record Multiple<TItem>(IReadOnlyList<TItem> Items) : SingleOrMultiple<TItem>;

public sealed record Empty<TItem>() : SingleOrMultiple<TItem>;
";

        var fixtest = $@"{TestData.Usings}

namespace Unions;

[Sundew.DiscriminatedUnions.DiscriminatedUnion]
public abstract record SingleOrMultiple<TItem>
{{
    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(Single<>))]
    public static SingleOrMultiple<TItem> Single(TItem item) => new Single<TItem>(item);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(Multiple<>))]
    public static SingleOrMultiple<TItem> Multiple(IReadOnlyList<TItem> items) => new Multiple<TItem>(items);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(Empty<>))]
    public static SingleOrMultiple<TItem> Empty() => new Empty<TItem>();
}}

public sealed record Single<TItem>(TItem Item) : SingleOrMultiple<TItem>;

public sealed record Multiple<TItem>(IReadOnlyList<TItem> Items) : SingleOrMultiple<TItem>;

public sealed record Empty<TItem>() : SingleOrMultiple<TItem>;
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(PopulateFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsRule)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("Unions.SingleOrMultiple<TItem>")
                .WithSpan(14, 24, 14, 40),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, true);
    }

    [Test]
    public async Task Given_DiscriminatedUnionWithNestedCases_Then_FactoryMethodsAreImplemented()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

[DiscriminatedUnion]
public abstract record Input
{{
    public sealed record IntInput(int Value) : Input;

    public sealed record DoubleInput(double Value) : Input;
}}
";

        var fixtest = $@"{TestData.Usings}

namespace Unions;

[DiscriminatedUnion]
public abstract record Input
{{
    public sealed record IntInput(int Value) : Input;

    public sealed record DoubleInput(double Value) : Input;

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(IntInput))]
    public static Input _IntInput(int value) => new IntInput(value);

    [Sundew.DiscriminatedUnions.CaseTypeAttribute(typeof(DoubleInput))]
    public static Input _DoubleInput(double value) => new DoubleInput(value);
}}
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(PopulateFactoryMethodsMarkerAnalyzer.PopulateFactoryMethodsRule)
                .WithSeverity(DiagnosticSeverity.Info)
                .WithArguments("Unions.Input")
                .WithSpan(14, 24, 14, 29),
        };

        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, true);
    }
}