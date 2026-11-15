// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageUnionAllCasesNotHandledCodeFixTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Development.Tests.SwitchStatement;

using System.Threading.Tasks;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Development.Tests.Verifiers.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionSwitchWarningSuppressor>;

/// <summary>
/// Tests the code fix that adds the cases a switch statement over a closed type or a union type does not handle.
/// </summary>
public class LanguageUnionAllCasesNotHandledCodeFixTests
{
    [Test]
    public async Task Given_SwitchStatementOverClosedType_When_CasesAreNotHandled_Then_ASectionShouldBeAddedForEachOfThem()
    {
        var test = @"#nullable enable
namespace Unions;

public closed record Shape;

public sealed record Circle(double Radius) : Shape;

public sealed record Square(double Side) : Shape;

public sealed record Polygon(int Sides) : Shape;

public class Tests
{
    public void Switch(Shape shape)
    {
        switch (shape)
        {
            case Circle circle:
                break;
        }
    }
}
";

        var fixtest = @"#nullable enable
namespace Unions;

public closed record Shape;

public sealed record Circle(double Radius) : Shape;

public sealed record Square(double Side) : Shape;

public sealed record Polygon(int Sides) : Shape;

public class Tests
{
    public void Switch(Shape shape)
    {
        switch (shape)
        {
            case Circle circle:
                break;
            case Square square:
                throw new System.NotImplementedException();
            case Polygon polygon:
                throw new System.NotImplementedException();
        }
    }
}
";
        var expected = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Square', 'Polygon'", Resources.Cases, "Unions.Shape", Resources.Are)
                .WithSpan(16, 9, 20, 10),
        };
        var expectedAfter = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Square")
                .WithSpan(21, 17, 21, 60),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Polygon")
                .WithSpan(23, 17, 23, 60),
        };

        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, expectedAfter);
    }

    [Test]
    public async Task Given_SwitchStatementOverUnion_When_CasesAndNullAreNotHandled_Then_ASectionShouldBeAddedForEachOfThemWithANameThatIsNotAKeyword()
    {
        var test = @"#nullable enable
namespace Unions;

public union Setting(int, string, bool?);

public class Tests
{
    public void Switch(Setting setting)
    {
        switch (setting)
        {
            case int number:
                break;
        }
    }
}
";

        var fixtest = @"#nullable enable
namespace Unions;

public union Setting(int, string, bool?);

public class Tests
{
    public void Switch(Setting setting)
    {
        switch (setting)
        {
            case int number:
                break;
            case string @string:
                throw new System.NotImplementedException();
            case bool boolean:
                throw new System.NotImplementedException();
            case null:
                throw new System.NotImplementedException();
        }
    }
}
";
        var expected = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'string', 'bool', 'null'", Resources.Cases, "Unions.Setting", Resources.Are)
                .WithSpan(10, 9, 14, 10),
        };
        var expectedAfter = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("string")
                .WithSpan(15, 17, 15, 60),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("bool")
                .WithSpan(17, 17, 17, 60),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("null")
                .WithSpan(19, 17, 19, 60),
        };

        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, expectedAfter);
    }
}
