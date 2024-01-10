// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllCasesNotHandledCodeFixTests.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.DiscriminatedUnions.Tests.SwitchExpression;

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sundew.DiscriminatedUnions.Analyzer;
using Sundew.DiscriminatedUnions.Tests.Verifiers;
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
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
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
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
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
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
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
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
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
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
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
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
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
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
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
        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
    }
}