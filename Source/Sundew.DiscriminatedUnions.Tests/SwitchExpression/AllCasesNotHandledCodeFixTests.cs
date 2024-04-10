// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllCasesNotHandledCodeFixTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Tests.SwitchExpression;

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Analyzer;
using VerifyCS = Sundew.DiscriminatedUnions.Tests.Verifiers.CSharpCodeFixVerifier<
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionsAnalyzer,
    Sundew.DiscriminatedUnions.CodeFixes.DiscriminatedUnionsCodeFixProvider,
    Sundew.DiscriminatedUnions.Analyzer.DiscriminatedUnionSwitchWarningSuppressor>;

[TestClass]
public class AllCasesNotHandledCodeFixTests
{
    [TestMethod]
    public async Task Given_SwitchExpression_When_NullableContextIsEnabledAndMultipleCasesAreNotHandled_Then_RemainingCasesWithoutNullShouldBeHandled()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public bool Switch(Result result)
    {{
        return result switch
        {{
            Result.Success success => true,
        }};
    }}
}}
{TestData.ValidResultUnion}
";

        var fixtest = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public bool Switch(Result result)
    {{
        return result switch
        {{
            Result.Success success => true,
            Result.Warning warning => throw new System.NotImplementedException(),
            Result.Error error => throw new System.NotImplementedException(),
        }};
    }}
}}
{TestData.ValidResultUnion}
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Warning', 'Error'", Resources.Cases, TestData.UnionsResult, Resources.Are)
                .WithSpan(18, 16, 21, 10),
        };
        var expectedAfter = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Warning")
                .WithSpan(21, 39, 21, 81),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Error")
                .WithSpan(22, 35, 22, 77),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, expectedAfter);
    }

    [TestMethod]
    public async Task Given_SwitchExpression_When_NullableContextIsEnabledAndCasesInBetweenIsNotHandled_Then_RemainingCaseShouldBeHandledInBetween()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public bool Switch(Result result)
    {{
        return result switch
        {{
            Result.Success success => true,
            Result.Error error => false,
        }};
    }}
}}
{TestData.ValidResultUnion}
";

        var fixtest = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public bool Switch(Result result)
    {{
        return result switch
        {{
            Result.Success success => true,
            Result.Warning warning => throw new System.NotImplementedException(),
            Result.Error error => false,
        }};
    }}
}}
{TestData.ValidResultUnion}
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Warning'", Resources.Case, TestData.UnionsResult, Resources.Is)
                .WithSpan(18, 16, 22, 10),
        };
        var expectedAfter = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Warning")
                .WithSpan(21, 39, 21, 81),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, expectedAfter);
    }

    [TestMethod]
    public async Task Given_SwitchExpression_When_NullableContextIsEnabledAndCasesInBetweenHasPattern_Then_RemainingCaseShouldBeHandledInBetweenAfterPattern()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public bool Switch(Result result)
    {{
        return result switch
        {{
            Result.Warning {{ Message: ""Some warning"" }} => false,
            Result.Error error => false,
        }};
    }}
}}
{TestData.ValidResultUnion}
";

        var fixtest = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public bool Switch(Result result)
    {{
        return result switch
        {{
            Result.Success success => throw new System.NotImplementedException(),
            Result.Warning {{ Message: ""Some warning"" }} => false,
            Result.Warning warning => throw new System.NotImplementedException(),
            Result.Error error => false,
        }};
    }}
}}
{TestData.ValidResultUnion}
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Success', 'Warning'", Resources.Cases, TestData.UnionsResult, Resources.Are)
                .WithSpan(18, 16, 22, 10),
        };

        var expectedAfter = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Success")
                .WithSpan(20, 39, 20, 81),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Warning")
                .WithSpan(22, 39, 22, 81),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, expectedAfter);
    }

    [TestMethod]
    public async Task Given_SwitchExpression_When_NullableContextIsEnabledAndNoCasesAreHandled_Then_AllCaseShouldBeHandled()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public bool Switch(Result result)
    {{
        return result switch
        {{
        }};
    }}
}}
{TestData.ValidResultUnion}
";

        var fixtest = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public bool Switch(Result result)
    {{
        return result switch
        {{
            Result.Success success => throw new System.NotImplementedException(),
            Result.Warning warning => throw new System.NotImplementedException(),
            Result.Error error => throw new System.NotImplementedException(),
        }};
    }}
}}
{TestData.ValidResultUnion}
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Success', 'Warning', 'Error'", Resources.Cases, TestData.UnionsResult, Resources.Are)
                .WithSpan(18, 16, 20, 10),
        };
        var expectedAfter = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Success")
                .WithSpan(20, 39, 20, 81),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Warning")
                .WithSpan(21, 39, 21, 81),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Error")
                .WithSpan(22, 35, 22, 77),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, expectedAfter);
    }

    [TestMethod]
    public async Task Given_SwitchExpression_When_NullableContextIsDisableAndMultipleCasesAreNotHandled_Then_RemainingCasesWithNullShouldBeHandled()
    {
        var test = $@"{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public bool Switch(Result result)
    {{
        return result switch
        {{
            Result.Success success => true,
        }};
    }}
}}
{TestData.ValidResultUnion}
";

        var fixtest = $@"{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public bool Switch(Result result)
    {{
        return result switch
        {{
            Result.Success success => true,
            Result.Warning warning => throw new System.NotImplementedException(),
            Result.Error error => throw new System.NotImplementedException(),
            null => throw new System.NotImplementedException(),
        }};
    }}
}}
{TestData.ValidResultUnion}
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Warning', 'Error', 'null'", Resources.Cases, TestData.UnionsResult, Resources.Are)
                .WithSpan(17, 16, 20, 10),
        };
        var expectedAfter = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Warning")
                .WithSpan(20, 39, 20, 81),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Error")
                .WithSpan(21, 35, 21, 77),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("null")
                .WithSpan(22, 21, 22, 63),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, expectedAfter);
    }

    [TestMethod]
    public async Task Given_SwitchExpression_When_UnionIsClosedGenericAndNullableContextIsEnableAndMultipleCasesAreNotHandled_Then_RemainingCasesShouldBeHandled()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public bool Switch(Option<string> textOption)
    {{
        return textOption switch
        {{
        }};
    }}
}}
{TestData.ValidGenericOptionUnion}
";

        var fixtest = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public bool Switch(Option<string> textOption)
    {{
        return textOption switch
        {{
            Option<string>.Some some => throw new System.NotImplementedException(),
            Option<string>.None none => throw new System.NotImplementedException(),
        }};
    }}
}}
{TestData.ValidGenericOptionUnion}
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Some', 'None'", Resources.Cases, TestData.UnionsOptionString, Resources.Are)
                .WithSpan(18, 16, 20, 10),
        };
        var expectedAfter = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Some")
                .WithSpan(20, 41, 20, 83),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("None")
                .WithSpan(21, 41, 21, 83),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, expectedAfter);
    }

    [TestMethod]
    public async Task Given_SwitchExpression_When_UnionIsOpenGenericAndNullableContextIsEnableAndMultipleCasesAreNotHandled_Then_RemainingCasesShouldBeHandled()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public bool Switch<T>(Option<T> textOption)
        where T : notnull
    {{
        return textOption switch
        {{
        }};
    }}
}}
{TestData.ValidGenericOptionUnion}
";

        var fixtest = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public bool Switch<T>(Option<T> textOption)
        where T : notnull
    {{
        return textOption switch
        {{
            Option<T>.Some some => throw new System.NotImplementedException(),
            Option<T>.None none => throw new System.NotImplementedException(),
        }};
    }}
}}
{TestData.ValidGenericOptionUnion}
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Some', 'None'", Resources.Cases, TestData.UnionsOptionT, Resources.Are)
                .WithSpan(19, 16, 21, 10),
        };
        var expectedAfter = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Some")
                .WithSpan(21, 36, 21, 78),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("None")
                .WithSpan(22, 36, 22, 78),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, expectedAfter);
    }

    [TestMethod]
    public async Task Given_SwitchExpression_When_UnionIsUnnestedClosedGenericAndNullableContextIsEnableAndMultipleCasesAreNotHandled_Then_RemainingCasesShouldBeHandled()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public bool Switch(ListCardinality<string> listCardinality)
    {{
        return listCardinality switch
        {{
        }};
    }}
}}
{TestData.ValidGenericListCardinalityUnion}
";

        var fixtest = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public bool Switch(ListCardinality<string> listCardinality)
    {{
        return listCardinality switch
        {{
            Empty<string> empty => throw new System.NotImplementedException(),
            Multiple<string> multiple => throw new System.NotImplementedException(),
            Single<string> single => throw new System.NotImplementedException(),
        }};
    }}
}}
{TestData.ValidGenericListCardinalityUnion}
";

        var expected = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Empty', 'Multiple', 'Single'", Resources.Cases, TestData.ListCardinalityString, Resources.Are)
                .WithSpan(18, 16, 20, 10),
        };
        var expectedAfter = new[]
        {
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Empty")
                .WithSpan(20, 36, 20, 78),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Multiple")
                .WithSpan(21, 42, 21, 84),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Single")
                .WithSpan(22, 38, 22, 80),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, expectedAfter);
    }

    [TestMethod]
    public async Task Given_SwitchExpression_When_UnionIsUnnestedClosedGenericAndNullableContextIsEnableAndMultipleCasesAreNotHandledAndReturnKeywordAndSemiColonIsMissing_Then_RemainingCasesShouldBeHandled()
    {
        var test = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public bool Switch(ListCardinality<string> listCardinality)
    {{
        listCardinality switch
        {{
        }}
    }}
}}
{TestData.ValidGenericListCardinalityUnion}
";

        var fixtest = $@"#nullable enable
{TestData.Usings}

namespace Unions;

public class DiscriminatedUnionSymbolAnalyzerTests
{{   
    public bool Switch(ListCardinality<string> listCardinality)
    {{
        listCardinality switch
        {{
            Empty<string> empty => throw new System.NotImplementedException(),
            Multiple<string> multiple => throw new System.NotImplementedException(),
            Single<string> single => throw new System.NotImplementedException(),
        }}
    }}
}}
{TestData.ValidGenericListCardinalityUnion}
";

        var notAllPathsReturnAValueDiagnostic = VerifyCS.Diagnostic(DiagnosticDescriptorHelper.Create(
                "CS0161",
                string.Empty,
                string.Empty,
                string.Empty,
                DiagnosticSeverity.Error,
                true,
                string.Empty))
            .WithArguments("Unions.DiscriminatedUnionSymbolAnalyzerTests.Switch(Unions.ListCardinality<string>)")
            .WithMessage("'DiscriminatedUnionSymbolAnalyzerTests.Switch(ListCardinality<string>)': not all code paths return a value")
            .WithSpan(16, 17, 16, 23);
        var noBestTypeWasFoundForTheSwitchExpressionDiagnostic = VerifyCS.Diagnostic(DiagnosticDescriptorHelper.Create(
                "CS8506",
                string.Empty,
                string.Empty,
                string.Empty,
                DiagnosticSeverity.Error,
                true,
                string.Empty))
            .WithMessage("No best type was found for the switch expression.")
            .WithSpan(18, 25, 18, 31);
        var semiColonExpectedDiagnostic = VerifyCS.Diagnostic(DiagnosticDescriptorHelper.Create(
                "CS1002",
                string.Empty,
                string.Empty,
                string.Empty,
                DiagnosticSeverity.Error,
                true,
                string.Empty))
            .WithMessage("; expected");
        var expected = new[]
        {
            notAllPathsReturnAValueDiagnostic,
            noBestTypeWasFoundForTheSwitchExpressionDiagnostic,
            semiColonExpectedDiagnostic.WithSpan(20, 10, 20, 10),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.SwitchAllCasesNotHandledRule)
                .WithArguments("'Empty', 'Multiple', 'Single'", Resources.Cases, TestData.ListCardinalityString, Resources.Are)
                .WithSpan(18, 9, 20, 10),
        };
        var expectedAfter = new[]
        {
            notAllPathsReturnAValueDiagnostic,
            noBestTypeWasFoundForTheSwitchExpressionDiagnostic,
            semiColonExpectedDiagnostic.WithSpan(23, 10, 23, 10),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Empty")
                .WithSpan(20, 36, 20, 78),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Multiple")
                .WithSpan(21, 42, 21, 84),
            VerifyCS.Diagnostic(DiscriminatedUnionsAnalyzer.CaseShouldBeImplementedRule)
                .WithArguments("Single")
                .WithSpan(22, 38, 22, 80),
        };
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest, expectedAfter);
    }
}